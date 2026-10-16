import { useMutation } from "@tanstack/react-query";
import { askQuestion } from "../api/endpoints";
import type { AskRequest, AskResponse } from "../api/types";

export function useAskQuestion() {
  return useMutation<AskResponse, unknown, AskRequest>({
    mutationFn: (data) => askQuestion(data),
  });
}
