import axiosInstance from '../utils/axiosInstance';
import { message } from 'antd';


axiosInstance.interceptors.response.use(
  response => response,
  error => {
    if (error.response?.status === 401) {
      message.error('Сессия устарела. Пожалуйста войдите заново в аккаунт');
      localStorage.removeItem('token');
      window.location.href = '/';
    } else if (error.response?.data?.message) {
      message.error(error.response.data.message);
    } else {
      message.error('Произошла ошибка');
    }
    return Promise.reject(error);
  }
);

