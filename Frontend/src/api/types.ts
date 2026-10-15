export interface UserProfile {
  id: string;
  nickname: string;
  email: string;
  avatarUrl: string;
  age: number;
  firstName: string;
  lastName: string;
  bio: string;
  country: string;
  city: string;
  createdAt: string;
}

export interface Document {
  id: string;
  fileName: string;
  fileSize: number;
  status: string;
  chunkCount: number;
  uploadedAt: string;
}

export interface SourceChunk {
  documentId: string;
  fileName: string;
  chunkIndex: number;
  preview: string;
  score: number;
}

export interface AskResponse {
  answer: string;
  sources: SourceChunk[];
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  nickname: string;
  email: string;
  password: string;
  avatarUrl: string;
  age: number;
  firstName: string;
  lastName: string;
  bio: string;
  country: string;
  city: string;
}

export interface UpdateProfileRequest {
  nickname?: string;
  avatarUrl?: string;
  age?: number;
  firstName?: string;
  lastName?: string;
  bio?: string;
  country?: string;
  city?: string;
}

export interface AskRequest {
  question: string;
  documentIds: string[] | null;
}
