export interface CategoryDto {
  categoryId: string;
  name: string;
  userId: string;
}

export interface NoteDto {
  noteId: string;
  title: string;
  content: string;
  categoryId: string;
  userId: string;
  reminderDate?: Date | null;
}

export interface UserDto {
  userId: string;
  username: string;
  email: string;
  password: string;
}

export interface CategoryReportDto {
  categoryId: string;
  categoryName: string;
  totalNotes: number;
  notesWithReminders: number;
  completedReminders: number;
  activeReminders: number;
  notes: NoteDto[];
}

export interface FullReportDto {
  categories: CategoryReportDto[];
  totalCategories: number;
  totalNotes: number;
  totalNotesWithReminders: number;
  totalCompletedReminders: number;
  totalActiveReminders: number;
}