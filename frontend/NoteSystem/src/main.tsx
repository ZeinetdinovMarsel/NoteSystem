import ReactDOM from 'react-dom/client';
import { ConfigProvider } from 'antd';
import App from './App';
import theme from './theme/themeConfig';
import './index.css';
import React from 'react';

ReactDOM.createRoot(document.getElementById('root')!).render(

  <React.StrictMode>
    <ConfigProvider theme={theme}>
      <App />
    </ConfigProvider>
  </React.StrictMode>
);