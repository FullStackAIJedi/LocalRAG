import { apiClient } from "./client";
import type {
  UserProfile,
  Document,
  AskResponse,
  LoginRequest,
  RegisterRequest,
  UpdateProfileRequest,
  AskRequest,
} from "./types";

export async function register(
  data: RegisterRequest,
): Promise<{ userId: string }> {
  const response = await apiClient.post("/auth/register", data);
  return response.data;
}

export async function login(data: LoginRequest): Promise<{ token: string }> {
  const response = await apiClient.post("/auth/login", data);
  return response.data;
}

export async function getMyProfile(): Promise<UserProfile> {
  const response = await apiClient.get("/profile/me");
  return response.data;
}

export async function updateMyProfile(
  data: UpdateProfileRequest,
): Promise<UserProfile> {
  const response = await apiClient.put("/profile/me", data);
  return response.data;
}

export async function getMyDocuments(): Promise<Document[]> {
  const response = await apiClient.get("/documents");
  return response.data;
}

export async function uploadDocument(
  file: File,
): Promise<{ documentId: string }> {
  const formData = new FormData();
  formData.append("file", file);

  const response = await apiClient.post("/documents/upload", formData, {
    headers: { "Content-Type": "multipart/form-data" },
  });
  return response.data;
}

export async function askQuestion(data: AskRequest): Promise<AskResponse> {
  const response = await apiClient.post("/ask", data);
  return response.data;
}
