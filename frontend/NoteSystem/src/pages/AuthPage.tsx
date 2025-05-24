import { useState } from 'react';
import { Button, Card, Form, Input, Tabs, message } from 'antd';
import { LockOutlined, UserOutlined, MailOutlined } from '@ant-design/icons';
import { useNavigate } from 'react-router-dom';
import axiosInstance from '../utils/axiosInstance';
import { useAuthStore } from '../services/authService';


type AuthType = 'login' | 'register';

export default function AuthPage() {
  const [loading, setLoading] = useState(false);
  const [type, setType] = useState<AuthType>('login');
  const navigate = useNavigate();
  const [form] = Form.useForm();
  const setToken = useAuthStore((state) => state.setToken);

  const onFinish = async (values: any) => {
    setLoading(true);
    try {
      const url = type === 'login' ? '/api/Users/login' : '/api/Users/register';
      const { data } = await axiosInstance.post(url, values);
      
      if (type === 'login') {
        setToken(data);
        navigate('/categories');
        message.success('Вход был выполнен успешно');
      } else {
        message.success('Регистрация была выполнена успешно');
        setType('login');
        form.resetFields();
      }
    }
    catch (error: any) {
      message.error(error.response?.data?.Detail || 'Произошла ошибка');
    }
    finally {
      setLoading(false);
    }
  };
  const items = [
      {
        key: 'login',
        label: 'Логин',
      },
      {
        key: 'register',
        label: 'Регистрация',
      },
    ];
    return (
    <Card>
       <Tabs 
        activeKey={type} 
        onChange={(key) => setType(key as AuthType)}
        items={items}
      />
      <Form form={form} onFinish={onFinish} layout="vertical">
        {type === 'register' && (
          <Form.Item 
            name="username" 
            label="Имя пользователя"
            rules={[
              { required: true, message: 'Пожалуйста введите имя пользователя' },
              { min: 3, message: 'Имя пользователя должно быть не короче 3 символов' }
            ]}
          >
            <Input prefix={<UserOutlined />} placeholder="Имя пользователя" />
          </Form.Item>
        )}

        <Form.Item 
          name="email" 
          label="Почта"
          rules={[
            { required: true, message: 'Пожалуйста введите почту' },
            { type: 'email', message: 'Пожалуйста введите корректную почту' }
          ]}
        >
          <Input prefix={<MailOutlined />} placeholder="Почта" />
        </Form.Item>

        <Form.Item 
          name="password" 
          label="Пароль"
          rules={[
            { required: true, message: 'Пожалуйста введите пароль' },
            { min: 6, message: 'Пароль должен состоять из не менее чем 6 символов' }
          ]}
        >
          <Input.Password prefix={<LockOutlined />} placeholder="Пароль" />
        </Form.Item>

        <Button 
          type="primary" 
          htmlType="submit" 
          loading={loading} 
          block
          size="large"
        >
          {type === 'login' ? 'Логин' : 'Регистрация'}
        </Button>
      </Form>
    </Card>
  );
}