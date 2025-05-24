import { RouterProvider, createBrowserRouter } from 'react-router-dom';
import AuthPage from './pages/AuthPage';
import CategoriesPage from './pages/CategoriesPage';
import NotesPage from './pages/NotesPage';
import NoteDetailPage from './pages/NoteDetailPage';
import ProtectedRoute from './components/ProtectedRoute';
import MainLayout from './components/MainLayout';

const router = createBrowserRouter([
  {
    path: '/',
    element: <AuthPage />,
  },
  {
    element: <MainLayout />,
    children: [
      {
        path: '/categories',
        element: <ProtectedRoute><CategoriesPage /></ProtectedRoute>,
      },
      {
        path: '/notes',
        element: <ProtectedRoute><NotesPage /></ProtectedRoute>,
      },
      {
        path: '/notes/:noteId',
        element: <ProtectedRoute><NoteDetailPage /></ProtectedRoute>,
      },
      {
        path: '/notes/new',
        element: <ProtectedRoute><NoteDetailPage noteId="new"/></ProtectedRoute>,
      },
    ],
  },
]);

export default function App() {
  return <RouterProvider router={router} />;
}