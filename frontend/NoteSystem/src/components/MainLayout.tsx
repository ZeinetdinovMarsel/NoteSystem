import { useEffect, useState } from 'react';
import { Outlet, Link, useLocation } from 'react-router-dom';
import { 
  Layout, 
  Menu, 
  theme, 
  Grid, 
  Avatar, 
  Dropdown, 
  Space, 
  Typography,
  Button
} from 'antd';
import { 
  LogoutOutlined, 
  UserOutlined, 
  MenuFoldOutlined, 
  MenuUnfoldOutlined,
} from '@ant-design/icons';
import  MyAvatar from '../assets/haramb.png';
import axiosInstance from '../utils/axiosInstance';

const { Header, Content, Sider } = Layout;
const { Text } = Typography;
const { useBreakpoint } = Grid;

export default function MainLayout() {
  const [collapsed, setCollapsed] = useState(false);
  const location = useLocation();
  const screens = useBreakpoint();
  const [userData, setUserData] = useState<{userName?: string, email?: string}>({});

  const fetchUserData = async () => {
    try {
      const response = await axiosInstance.get('/api/users/me');
      setUserData(response.data);
    } catch (error) {
      console.error('Failed to fetch user data:', error);
    }
  };

  useEffect(() => {
    fetchUserData();
  }, []);

  const {
    token: { colorBgContainer, colorPrimary },
  } = theme.useToken();

  const getSelectedKeys = () => {
    if (location.pathname.startsWith('/categories')) return ['categories'];
    if (location.pathname.startsWith('/notes')) return ['notes'];
    return [];
  };

  return (
    <Layout style={{ minHeight: '100vh', width: '100vw' }}>
      {!screens.lg && (
        <Button
          type="text"
          icon={collapsed ? <MenuUnfoldOutlined /> : <MenuFoldOutlined />}
          onClick={() => setCollapsed(!collapsed)}
          style={{
            position: 'fixed',
            zIndex: 100,
            top: 16,
            left: 16,
            fontSize: '16px',
            width: 48,
            height: 48,
            color: colorPrimary,
          }}
        />
      )}

      <Sider
        width={250}
        collapsedWidth={screens.lg ? 80 : 0}
        collapsible
        collapsed={collapsed}
        onCollapse={setCollapsed}
        breakpoint="lg"
        trigger={null}
        style={{
          position: screens.lg ? 'fixed' : 'absolute',
          height: '100vh',
          zIndex: 99,
          overflow: 'auto',
        }}
      >
        <div style={{
          height: 64,
           backgroundColor:'white',
           border: '5px solid #001529',
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'center',
        }}>
          {collapsed ? (
            <span style={{ fontSize: 24 }}>📝</span>
          ) : (
            <Text strong style={{ fontSize: 18}}>Система заметок</Text>
          )}
        </div>

        <Menu
          theme="dark"
          mode="inline"
          selectedKeys={getSelectedKeys()}
          items={[
            {
              key: 'categories',
              icon: <span>📁</span>,
              label: <Link to="/categories">Категории</Link>,
            },
            {
              key: 'notes',
              icon: <span>📝</span>,
              label: <Link to="/notes">Замтеки</Link>,
            },
          ]}
          style={{ borderRight: 0 }}
        />
      </Sider>
      
      <Layout 
        style={{ 
          marginLeft: screens.lg ? (collapsed ? 80 : 250) : 0,
          width: screens.lg ? (collapsed ? 'calc(100vw - 80px)' : 'calc(100vw - 250px)') : '100vw',
          transition: 'all 0.2s',
          minHeight: '100vh',
        }}
      >
        <Header style={{ 
          padding: screens.xs ? '0 12px' : '0 24px',
          background: colorBgContainer,
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'flex-end',
          position: 'sticky',
          top: 0,
          zIndex: 98,
          width: '100%',
        }}>
          <Space size="large">

            <Dropdown 
              menu={{ 
                items: [
                  {
                    key: 'logout',
                    label: 'Выйти',
                    icon: <LogoutOutlined />,
                    onClick: () => {
                      localStorage.removeItem('token');
                      window.location.href = '/';
                    }
                  }
                ]
              }} 
              trigger={['click']}
            >
              <Space style={{ cursor: 'pointer' }}>
                <Avatar 
                  src={MyAvatar}
                  style={{ backgroundColor: colorPrimary }}
                />
                {!collapsed && (
                  <Text strong>{userData.userName || userData.email || 'User'}</Text>
                )}
              </Space>
            </Dropdown>
          </Space>
        </Header>

        <Content
          style={{
            padding: screens.xs ? 12 : 24,
            minHeight: 'calc(100vh - 64px)',
            background: colorBgContainer,
          }}
        >
          <div style={{ 
            background: colorBgContainer,
            borderRadius: 8,
            height: '100%',
            padding: screens.xs ? 12 : 24,
          }}>
            <Outlet />
          </div>
        </Content>
      </Layout>
    </Layout>
  );
}