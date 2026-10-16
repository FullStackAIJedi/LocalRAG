import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { getMyDocuments, uploadDocument } from "../api/endpoints";
import { queryKeys } from "./queryKeys";

export function useMyDocuments() {
  return useQuery({
    queryKey: queryKeys.documents.my,
    queryFn: getMyDocuments,
  });
}

export function useUploadDocument() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (file: File) => uploadDocument(file),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: queryKeys.documents.my });
    },
  });
}
