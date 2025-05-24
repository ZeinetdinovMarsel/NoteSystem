import { Button } from 'antd';
import { LogoutOutlined } from '@ant-design/icons';
import { useNavigate } from 'react-router-dom';

export default function LogoutButton() {
  const navigate = useNavigate();

  const handleLogout = () => {
    localStorage.removeItem('token');
    navigate('/');
  };

  return (
    <Button 
      type="text" 
      icon={<LogoutOutlined />} 
      onClick={handleLogout}
    >
      Logout
    </Button>
  );
}