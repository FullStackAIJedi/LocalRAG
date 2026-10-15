import { useState, useEffect } from "react";
import type { FormEvent } from "react";
import { useNavigate } from "react-router-dom";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { getMyProfile, updateMyProfile } from "../api/endpoints";
import type { UpdateProfileRequest } from "../api/types";
import { useAuth } from "../context/AuthContext";

export default function EditProfilePage() {
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const { refreshUser } = useAuth();
  const [error, setError] = useState("");
  const [form, setForm] = useState<UpdateProfileRequest>({});

  const { data: profile, isLoading } = useQuery({
    queryKey: ["myProfile"],
    queryFn: getMyProfile,
  });

  useEffect(() => {
    if (profile) {
      setForm({
        nickname: profile.nickname,
        avatarUrl: profile.avatarUrl,
        age: profile.age,
        firstName: profile.firstName,
        lastName: profile.lastName,
        bio: profile.bio,
        country: profile.country,
        city: profile.city,
      });
    }
  }, [profile]);

  const updateMutation = useMutation({
    mutationFn: updateMyProfile,
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["myProfile"] });
      await refreshUser();
      navigate("/profile");
    },
    onError: (err: any) => {
      setError(err.response?.data?.error || "Failed to update profile.");
    },
  });

  const handleSubmit = (e: FormEvent) => {
    e.preventDefault();
    setError("");
    updateMutation.mutate(form);
  };

  const update = (field: keyof UpdateProfileRequest, value: any) => {
    setForm((prev) => ({ ...prev, [field]: value }));
  };

  if (isLoading) return <div className="loading">Loading...</div>;

  return (
    <div className="edit-profile-page">
      <h1>Edit Profile</h1>
      <form onSubmit={handleSubmit} className="edit-form">
        <div className="form-group">
          <label>Nickname</label>
          <input
            value={form.nickname || ""}
            onChange={(e) => update("nickname", e.target.value)}
          />
        </div>

        <div className="form-group">
          <label>Avatar URL</label>
          <input
            value={form.avatarUrl || ""}
            onChange={(e) => update("avatarUrl", e.target.value)}
          />
        </div>

        <div className="form-row">
          <div className="form-group">
            <label>First Name</label>
            <input
              value={form.firstName || ""}
              onChange={(e) => update("firstName", e.target.value)}
            />
          </div>
          <div className="form-group">
            <label>Last Name</label>
            <input
              value={form.lastName || ""}
              onChange={(e) => update("lastName", e.target.value)}
            />
          </div>
        </div>

        <div className="form-row">
          <div className="form-group">
            <label>Age</label>
            <input
              type="number"
              value={form.age || ""}
              onChange={(e) => update("age", parseInt(e.target.value) || 0)}
            />
          </div>
          <div className="form-group">
            <label>Country</label>
            <input
              value={form.country || ""}
              onChange={(e) => update("country", e.target.value)}
            />
          </div>
          <div className="form-group">
            <label>City</label>
            <input
              value={form.city || ""}
              onChange={(e) => update("city", e.target.value)}
            />
          </div>
        </div>

        <div className="form-group">
          <label>Bio</label>
          <textarea
            value={form.bio || ""}
            onChange={(e) => update("bio", e.target.value)}
            rows={4}
          />
        </div>

        {error && <div className="error">{error}</div>}

        <div className="form-actions">
          <button type="submit" disabled={updateMutation.isPending}>
            {updateMutation.isPending ? "Saving..." : "Save"}
          </button>
          <button
            type="button"
            onClick={() => navigate("/profile")}
            className="btn-secondary"
          >
            Cancel
          </button>
        </div>
      </form>
    </div>
  );
}
