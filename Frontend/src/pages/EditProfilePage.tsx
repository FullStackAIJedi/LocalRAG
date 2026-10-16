import { useState, useEffect } from "react";
import type { FormEvent } from "react";
import { useNavigate } from "react-router-dom";
import { useProfile, useUpdateProfile } from "../hooks/useProfile";
import { useAuth } from "../context/AuthContext";
import type { UpdateProfileRequest } from "../api/types";
import AvatarCircle from "../components/AvatarCircle";

export default function EditProfilePage() {
  const navigate = useNavigate();
  const { refreshUser } = useAuth();
  const [error, setError] = useState("");
  const [form, setForm] = useState<UpdateProfileRequest>({});

  const { data: profile, isLoading } = useProfile();
  const updateMutation = useUpdateProfile();

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

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setError("");

    try {
      await updateMutation.mutateAsync(form);
      await refreshUser();
      navigate("/profile");
    } catch (err: any) {
      setError(err.response?.data?.error || "Failed to update profile.");
    }
  };

  const update = (field: keyof UpdateProfileRequest, value: any) => {
    setForm((prev) => ({ ...prev, [field]: value }));
  };

  if (isLoading) return <div className="loading">Loading...</div>;

  return (
    <div className="edit-profile-page">
      <h1>Edit Profile</h1>

      <AvatarCircle
        imageUrl={form.avatarUrl || undefined}
        fallbackText={form.nickname}
      />

      <form onSubmit={handleSubmit}>
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
