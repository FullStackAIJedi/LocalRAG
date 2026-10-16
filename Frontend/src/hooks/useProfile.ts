import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { getMyProfile, updateMyProfile } from "../api/endpoints";
import type { UpdateProfileRequest } from "../api/types";
import { queryKeys } from "./queryKeys";

export function useProfile() {
  return useQuery({
    queryKey: queryKeys.profile.me,
    queryFn: getMyProfile,
  });
}

export function useUpdateProfile() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data: UpdateProfileRequest) => updateMyProfile(data),
    onSuccess: (updated) => {
      queryClient.setQueryData(queryKeys.profile.me, updated);
    },
  });
}
