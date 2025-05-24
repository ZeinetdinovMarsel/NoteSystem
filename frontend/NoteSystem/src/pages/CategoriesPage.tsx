import { useState, useEffect, useCallback } from 'react';
import { 
  Button, 
  Table, 
  Modal, 
  Form, 
  Input, 
  message, 
  Popconfirm,
  Space,
  Grid,
  Typography
} from 'antd';
import { 
  PlusOutlined, 
  EditOutlined, 
  DeleteOutlined, 
  CheckSquareOutlined,
  CloseSquareOutlined
} from '@ant-design/icons';

import type { CategoryDto } from '../types';
import axiosInstance from '../utils/axiosInstance';

const { useBreakpoint } = Grid;
  const { Text } = Typography;
export default function CategoriesPage() {
  const [categories, setCategories] = useState<CategoryDto[]>([]);
  const [selectedRowKeys, setSelectedRowKeys] = useState<React.Key[]>([]);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [form] = Form.useForm();
  const [editingId, setEditingId] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);
  const screens = useBreakpoint();

  useEffect(() => {
    loadCategories();
  }, []);

  const loadCategories = async () => {
    setLoading(true);
    try {
      const { data } = await axiosInstance.get('/categories');
      setCategories(data);
      setSelectedRowKeys([]);
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id: string) => {
    try {
      await axiosInstance.delete(`/categories/${id}`);
      message.success('Категория удалена');
      loadCategories();
    } catch (error) {
      message.error(error.response?.data?.Detail || 'Не удалось удалить категорию');
    }
  };

  const handleBatchDelete = async () => {
    try {
      await Promise.all(
        selectedRowKeys.map(id => 
          axiosInstance.delete(`/categories/${id}`)
        )
      );
      message.success(`Удалено ${selectedRowKeys.length} категорий`);
      loadCategories();
    } catch (error) {
      message.error(error.response?.data?.Detail || 'Не удалось удалить категории');
    }
  };

  const handleSubmit = async (values: any) => {
    try {
      if (editingId) {
        const updatedValues = { ...values, categoryId: editingId };
        await axiosInstance.put(`/categories`, updatedValues);
        message.success('Категория обновлена');
      } else {
        await axiosInstance.post('/categories', values);
        message.success('Категория создана');
      }
      loadCategories();
      setIsModalOpen(false);
      form.resetFields();
    } catch (error) {
      message.error(error.response?.data?.Detail || 'Ошибка при сохранении категории');
    }
  };

  const onSelectChange = (newSelectedRowKeys: React.Key[]) => {
    setSelectedRowKeys(newSelectedRowKeys);
  };

  const selectAll = useCallback(() => {
    setSelectedRowKeys(categories.map(item => item.categoryId));
  }, [categories]);

  const unselectAll = useCallback(() => {
    setSelectedRowKeys([]);
  }, []);

  const rowSelection = {
    selectedRowKeys,
    onChange: onSelectChange,
  };

  const columns = [
    {
      title: 'Название',
      dataIndex: 'name',
      key: 'name',
      width: 300,
    },
    {
      title: 'Действия',
      key: 'actions',
      width: 200,
      render: (_: any, record: CategoryDto) => (
        <Space>
          <Button
            icon={<EditOutlined />}
            onClick={() => {
              setEditingId(record.categoryId);
              form.setFieldsValue(record);
              setIsModalOpen(true);
            }}
          />
          <Popconfirm
            title="Удалить эту категорию?"
            description="Все заметки в этой категории также будут удалены"
            onConfirm={() => handleDelete(record.categoryId)}
            okText="Да"
            cancelText="Нет"
          >
            <Button danger icon={<DeleteOutlined />} />
          </Popconfirm>
        </Space>
      ),
    },
  ];

  return (
    <div style={{ padding: screens.xs ? 12 : 24 }}>
      <Space style={{ marginBottom: 16 }}>
        <Button
          type="primary"
          icon={<PlusOutlined />}
          onClick={() => {
            setEditingId(null);
            form.resetFields();
            setIsModalOpen(true);
          }}
        >
          Добавить категорию
        </Button>
        <Text strong style={{ marginLeft: 8 }}>
          Всего категорий: {categories.length}
          {selectedRowKeys.length > 0 && ` (Выбрано: ${selectedRowKeys.length})`}
        </Text>

        {selectedRowKeys.length > 0 && (
          <>
            <Popconfirm
              title={`Удалить ${selectedRowKeys.length} выбранные категории?`}
              onConfirm={handleBatchDelete}
              okText="Да"
              cancelText="Нет"
            >
              <Button danger icon={<DeleteOutlined />}>
                Удалить выбранные ({selectedRowKeys.length})
              </Button>
            </Popconfirm>
            <Button icon={<CheckSquareOutlined />} onClick={selectAll}>
              Выбрать все
            </Button>
            <Button icon={<CloseSquareOutlined />} onClick={unselectAll}>
              Очистить выбор
            </Button>
          </>
        )}
      </Space>

      <Table
        rowSelection={rowSelection}
        dataSource={categories}
        columns={columns}
        rowKey="categoryId"
        loading={loading}
        scroll={{ x: true }}
      />

      <Modal
        title={editingId ? 'Редактировать категорию' : 'Новая категория'}
        open={isModalOpen}
        onCancel={() => {
          setIsModalOpen(false);
          form.resetFields();
          setEditingId(null);
        }}
        onOk={form.submit}
        destroyOnHidden
        forceRender
      >
        <Form form={form} onFinish={handleSubmit} layout="vertical">
          <Form.Item
            name="name"
            label="Название категории"
            rules={[
              { required: true, message: 'Пожалуйста, введите название категории!' },
              { max: 50, message: 'Название должно быть меньше 50 символов' }
            ]}
          >
            <Input />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
}