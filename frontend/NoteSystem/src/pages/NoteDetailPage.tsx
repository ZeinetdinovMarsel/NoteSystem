import { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import {
  Button,
  Input,
  DatePicker,
  message,
  Select,
  Space,
  Popconfirm,
  Form,
  Typography,
  Grid
} from 'antd';
import ReactQuill from 'react-quill';
import 'react-quill/dist/quill.snow.css';
import type { CategoryDto } from '../types';
import dayjs from 'dayjs';
import axiosInstance from '../utils/axiosInstance';

const { useBreakpoint } = Grid;
const { Title } = Typography;

const modules = {
  toolbar: [
    [{ header: [1, 2, 3, false] }],
    ['bold', 'italic', 'underline', 'strike'],
    [{ color: [] }, { background: [] }],
    [{ list: 'ordered' }, { list: 'bullet' }],
  ],
};

export default function NoteDetailPage({ noteId: propNoteId }: { noteId?: string }) {
  const { noteId: paramNoteId } = useParams();
  const noteId = propNoteId || paramNoteId;
  const navigate = useNavigate();
  const [categories, setCategories] = useState<CategoryDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const screens = useBreakpoint();
  const [form] = Form.useForm();

  useEffect(() => {
    const fetchData = async () => {
      try {
        setLoading(true);
        const categoriesResponse = await axiosInstance.get('/categories');
        setCategories(categoriesResponse.data);

        if (noteId && noteId !== 'new') {
          const noteResponse = await axiosInstance.get(`/notes/${noteId}`);
          form.setFieldsValue({
            title: noteResponse.data.title,
            content: noteResponse.data.content,
            categoryId: noteResponse.data.categoryId,
            reminderDate: noteResponse.data.reminderDate
              ? dayjs(noteResponse.data.reminderDate)
              : null
          });
        } else if (noteId === 'new') {
          form.setFieldsValue({
            title: '',
            content: '',
            categoryId: undefined,
            reminderDate: null
          });
        }
      } catch (error) {
        message.error(error.response?.data?.Detail || 'Не удалось загрузить данные');
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [noteId, form]);

  const handleSave = async () => {
    try {
      setSaving(true);
      const values = await form.validateFields();

      const noteData = {
        title: values.title,
        content: values.content,
        categoryId: values.categoryId,
        reminderDate: values.reminderDate
          ? values.reminderDate.toISOString()
          : null
      };

      if (noteId == 'new') {
        await axiosInstance.post('/notes', noteData);
        message.success('Заметка успешно создана!');
      } else {
        const updatedValues = { ...noteData, noteId: noteId };
        await axiosInstance.put(`/notes`, updatedValues);
        message.success('Заметка успешно обновлена!');
      }

      navigate('/notes');
    } catch (error) {
      message.error(error.response?.data?.Detail || 'Ошибка при сохранении заметки');
    } finally {
      setSaving(false);
    }
  };

  const handleDelete = async () => {
    try {
      await axiosInstance.delete(`/notes/${noteId}`);
      message.success('Заметка успешно удалена!');
      navigate('/notes');
    } catch (error) {
      message.error(error.response?.data?.Detail || 'Не удалось удалить заметку');
    }
  };

  return (
    <div style={{
      padding: screens.xs ? 12 : 24,
      maxWidth: 800,
      margin: '0 auto'
    }}>
      <Title level={3} style={{ textAlign: 'center', marginBottom: 24 }}>
        {noteId === 'new' ? 'Создать новую заметку' : 'Редактировать заметку'}
      </Title>
      <Form
        form={form}
        layout="vertical"
        initialValues={{
          title: '',
          content: '',
          categoryId: undefined,
          reminderDate: null
        }}
      >
        <Form.Item
          name="title"
          label="Заголовок"
          rules={[
            { required: true, message: 'Пожалуйста, введите заголовок заметки' },
            { max: 100, message: 'Слишком длинный заголовок' }
          ]}
        >
          <Input placeholder="Введите заголовок заметки" />
        </Form.Item>

        <Form.Item
          name="categoryId"
          label="Категория"
          rules={[{ required: true, message: 'Пожалуйста, выберите категорию' }]}
        >
          <Select
            placeholder="Выберите категорию"
            loading={loading}
            options={categories.map(category => ({
              label: category.name,
              value: category.categoryId
            }))}
          />
        </Form.Item>

        <Form.Item name="reminderDate" label="Напоминание">
          <DatePicker
            showTime
            style={{ width: '100%' }}
            format="YYYY-MM-DD HH:mm"
          />
        </Form.Item>

        <Form.Item
          name="content"
          label="Содержание"
          rules={[{ required: true, message: 'Пожалуйста, введите содержание' }]}
        >
          <ReactQuill
            theme="snow"
            modules={modules}
            style={{ height: 300 }}
            onChange={(content) => form.setFieldsValue({ content })}
          />
        </Form.Item>

        <Space>
          <Button
            type="primary"
            onClick={handleSave}
            loading={saving}
            size="large"
          >
            Сохранить
          </Button>

          {noteId !== 'new' && (
            <Popconfirm
              title="Вы уверены, что хотите удалить эту заметку?"
              onConfirm={handleDelete}
              okText="Да"
              cancelText="Нет"
            >
              <Button danger size="large">
                Удалить
              </Button>
            </Popconfirm>
          )}

          <Button
            onClick={() => navigate('/notes')}
            size="large"
          >
            Отмена
          </Button>
        </Space>
      </Form>
    </div>
  );
}

