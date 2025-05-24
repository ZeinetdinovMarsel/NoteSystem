import { useState, useEffect, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Button,
  Select,
  List,
  Card,
  message,
  Grid,
  Empty,
  Popconfirm,
  Space,
  Checkbox,
  Badge,
  Typography,
  Table,
  Modal,
  Tag,
  Statistic,
  Row,
  Col
} from 'antd';
import {
  PlusOutlined,
  DeleteOutlined,
  CheckSquareOutlined,
  CloseSquareOutlined,
  FileTextOutlined,
  MinusOutlined
} from '@ant-design/icons';
import type { NoteDto, CategoryDto, FullReportDto, CategoryReportDto } from '../types';
import axiosInstance from '../utils/axiosInstance';

const { useBreakpoint } = Grid;
const { Title, Text } = Typography;
const { Column } = Table;

export default function NotesPage() {
  const [notes, setNotes] = useState<NoteDto[]>([]);
  const [categories, setCategories] = useState<CategoryDto[]>([]);
  const [selectedCategory, setSelectedCategory] = useState<string>('');
  const [selectedNotes, setSelectedNotes] = useState<string[]>([]);
  const [loading, setLoading] = useState(false);
  const [report, setReport] = useState<FullReportDto | null>(null);
  const [reportModalVisible, setReportModalVisible] = useState(false);
  const [reportLoading, setReportLoading] = useState(false);
  const navigate = useNavigate();
  const screens = useBreakpoint();

  const loadData = async () => {
    setLoading(true);
    try {
      const [notesRes, categoriesRes] = await Promise.all([
        axiosInstance.get('/notes'),
        axiosInstance.get('/categories'),
      ]);
      setNotes(notesRes.data);
      setCategories(categoriesRes.data);
      setSelectedNotes([]);
    } catch (error) {
      message.error(error.response?.data?.Detail);
    } finally {
      setLoading(false);
    }
  };

  const loadReport = async () => {
    setReportLoading(true);
    try {
      const response = await axiosInstance.get('/report');
      setReport(response.data);
      setReportModalVisible(true);
    } catch (error) {
      message.error('Не удалось загрузить отчет');
    } finally {
      setReportLoading(false);
    }
  };

  const handleBatchDelete = async () => {
    try {
      await Promise.all(
        selectedNotes.map(id =>
          axiosInstance.delete(`/notes/${id}`)
        )
      );
      message.success(`Удалено ${selectedNotes.length} заметок`);
      loadData();
    } catch (error) {
      message.error(error.response?.data?.Detail || 'Не удалось удалить заметки');
    }
  };

  const toggleNoteSelection = (noteId: string) => {
    setSelectedNotes(prev =>
      prev.includes(noteId)
        ? prev.filter(id => id !== noteId)
        : [...prev, noteId]
    );
  };

  const selectAll = useCallback(() => {
    const filtered = selectedCategory
      ? notes.filter(note => note.categoryId === selectedCategory)
      : notes;
    setSelectedNotes(filtered.map(note => note.noteId));
  }, [notes, selectedCategory]);

  const unselectAll = useCallback(() => {
    setSelectedNotes([]);
  }, []);

  const filteredNotes = selectedCategory
    ? notes.filter(note => note.categoryId === selectedCategory)
    : notes;

  const gridProps = screens.xxl
    ? { column: 4 }
    : screens.xl
      ? { column: 3 }
      : screens.lg
        ? { column: 2 }
        : screens.md
          ? { column: 2 }
          : { column: 1 };

  useEffect(() => {
    loadData();
  }, []);

  return (
    <div style={{ padding: screens.xs ? 12 : 24 }}>
      <div style={{
        display: 'flex',
        gap: 16,
        marginBottom: 24,
        flexDirection: screens.xs ? 'column' : 'row'
      }}>
        <Select
          placeholder="Выберите категорию"
          style={{ minWidth: 200 }}
          onChange={value => setSelectedCategory(value as string)}
          options={[
            { label: 'Все категории', value: '' },
            ...categories.map(c => ({
              label: c.name,
              value: c.categoryId,
            })),
          ]}
          loading={loading}
        />

        <Space>
          <Button
            type="primary"
            icon={<PlusOutlined />}
            onClick={() => navigate('/notes/new')}
          >
            Новая заметка
          </Button>
          <Button
            icon={<FileTextOutlined />}
            onClick={loadReport}
          >
            Отчет по категориям
          </Button>
          <Text strong style={{ marginLeft: 8 }}>
            {`Всего Заметок: ${filteredNotes.length}`}
            {selectedNotes.length > 0 && ` (Выбрано: ${selectedNotes.length})`}
          </Text>
          {selectedNotes.length > 0 && (
            <>
              <Popconfirm
                title={`Удалить ${selectedNotes.length} выбранных заметок?`}
                onConfirm={handleBatchDelete}
                okText="Да"
                cancelText="Нет"
              >
                <Button danger icon={<DeleteOutlined />}>
                  Удалить ({selectedNotes.length})
                </Button>
              </Popconfirm>
              <Button icon={<CheckSquareOutlined />} onClick={selectAll}>
                Выбрать все
              </Button>
              <Button icon={<CloseSquareOutlined />} onClick={unselectAll}>
                Очистить
              </Button>
            </>
          )}
        </Space>
      </div>

      {filteredNotes.length === 0 ? (
        <Empty description="Заметки не найдены" />
      ) : (
        <List
          grid={{ gutter: 20, ...gridProps }}
          dataSource={filteredNotes}
          loading={loading}
          renderItem={note => (
            <List.Item>
              <Card
                title={
                  <Space>
                    <Checkbox
                      checked={selectedNotes.includes(note.noteId)}
                      onChange={() => toggleNoteSelection(note.noteId)}
                      onClick={e => e.stopPropagation()}
                    />
                    <Title level={3}>{note.title}</Title>
                  </Space>
                }
                hoverable
                onClick={() => navigate(`/notes/${note.noteId}`)}
                style={{
                  height: '100%',
                  border: selectedNotes.includes(note.noteId)
                    ? '2px solid #1890ff'
                    : undefined
                }}
                extra={
                  note.reminderDate && (
                    <Badge
                      color={new Date(note.reminderDate) < new Date() ? 'green' : 'orange'}
                      dot
                    />
                  )
                }
              >
                <div style={{
                  overflow: 'hidden',
                  textOverflow: 'ellipsis',
                  display: '-webkit-box',
                  WebkitLineClamp: 3,
                  WebkitBoxOrient: 'vertical'
                }} dangerouslySetInnerHTML={{ __html: note.content.substring(0, 100) }} />
              </Card>
            </List.Item>
          )}
        />
      )}

      <Modal
        title="Отчет по категориям"
        width={800}
        visible={reportModalVisible}
        onCancel={() => setReportModalVisible(false)}
        footer={null}
      >
        {reportLoading ? (
          <Empty description="Загрузка отчета..." />
        ) : report ? (
          <>
            <Row gutter={16} style={{ marginBottom: 24 }}>
              <Col span={6}>
                <Statistic title="Всего категорий" value={report.totalCategories} />
              </Col>
              <Col span={6}>
                <Statistic title="Всего заметок" value={report.totalNotes} />
              </Col>
              <Col span={6}>
                <Statistic title="Заметок с напоминаниями" value={report.totalNotesWithReminders} />
              </Col>
              <Col span={6}>
                <Statistic title="Активных напоминаний" value={report.totalActiveReminders} />
              </Col>
            </Row>

            <Table
              dataSource={report.categories}
              rowKey="categoryId"
              pagination={false}
              scroll={{ y: 400 }}
              expandable={{
                expandedRowRender: (record: CategoryReportDto) => (
                  <div style={{ margin: 0 }}>
                    <Text strong>Заметки в категории:</Text>
                    <List
                      size="small"
                      dataSource={record.notes}
                      renderItem={note => (
                        <List.Item>
                          <Space>
                            {note.reminderDate && (
                              <Badge
                                color={new Date(note.reminderDate) < new Date() ? 'green' : 'orange'}
                                dot
                              />
                            )}
                            <Text>{note.title}</Text>
                            {note.reminderDate && (
                              <Text type="secondary">
                                {new Date(note.reminderDate).toLocaleString()}
                              </Text>
                            )}
                          </Space>
                        </List.Item>
                      )}
                    />
                  </div>
                ),
                rowExpandable: (record) => record.notes.length > 0,
                expandIcon: ({ expanded, onExpand, record }) =>
                  record.notes.length > 0 ? (
                    <Button
                      type="text"
                      size="small"
                      icon={expanded ? <MinusOutlined /> : <PlusOutlined />}
                      onClick={e => {
                        e.stopPropagation();
                        onExpand(record, e);
                      }}
                    />
                  ) : null
              }}
            >
              <Column
                title="Категория"
                dataIndex="categoryName"
                key="categoryName"
                render={(text, record: CategoryReportDto) => (
                  <Text strong>{text}</Text>
                )}
              />
              <Column
                title="Всего заметок"
                dataIndex="totalNotes"
                key="totalNotes"
                align="center"
              />
              <Column
                title="С напоминаниями"
                dataIndex="notesWithReminders"
                key="notesWithReminders"
                align="center"
                render={(value) => (
                  <Tag color={value > 0 ? 'blue' : 'default'}>{value}</Tag>
                )}
              />
              <Column
                title="Завершенные напоминания"
                dataIndex="completedReminders"
                key="completedReminders"
                align="center"
                render={(value) => (
                  <Tag color={value > 0 ? 'green' : 'default'}>{value}</Tag>
                )}
              />
              <Column
                title="Активные напоминания"
                dataIndex="activeReminders"
                key="activeReminders"
                align="center"
                render={(value) => (
                  <Tag color={value > 0 ? 'orange' : 'default'}>{value}</Tag>
                )}
              />
            </Table>
          </>
        ) : (
          <Empty description="Данные отчета не загружены" />
        )}
      </Modal>
    </div>
  );
}