import { useMutation } from "@tanstack/react-query";
import { login, register, logout } from "../api/endpoints";
import type { LoginRequest, RegisterRequest } from "../api/types";

export function useLogin() {
  return useMutation({
    mutationFn: (data: LoginRequest) => login(data),
  });
}

export function useRegister() {
  return useMutation({
    mutationFn: (data: RegisterRequest) => register(data),
  });
}

export function useLogout() {
  return useMutation({
    mutationFn: () => logout(),
  });
}
