export interface AppError {
  status: number;
  userMessage: string;
  fieldErrors?: Record<string, string[]>;
}
