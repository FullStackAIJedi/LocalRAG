import { useQuery } from "@tanstack/react-query";
import { Link } from "react-router-dom";
import { getMyProfile } from "../api/endpoints";

export default function ProfilePage() {
  const {
    data: profile,
    isLoading,
    error,
  } = useQuery({
    queryKey: ["myProfile"],
    queryFn: getMyProfile,
  });

  if (isLoading) return <div className="loading">Loading profile...</div>;

  if (error) return <div className="error">Failed to load profile.</div>;

  if (!profile) return null;

  return (
    <div className="profile-page">
      <div className="profile-header">
        {profile.avatarUrl ? (
          <img src={profile.avatarUrl} alt="Avatar" className="avatar-large" />
        ) : (
          <div className="avatar-placeholder">
            {profile.nickname.charAt(0).toUpperCase()}
          </div>
        )}

        <div className="profile-info">
          <h1>{profile.nickname}</h1>
          <p className="profile-name">
            {profile.firstName} {profile.lastName}
          </p>
          <p className="profile-location">
            {profile.city}, {profile.country}
          </p>
        </div>

        <Link to="/profile/edit" className="btn-edit">
          Edit Profile
        </Link>
      </div>

      <div className="profile-details">
        <div className="detail-row">
          <span className="label">Email:</span>
          <span>{profile.email}</span>
        </div>

        <div className="detail-row">
          <span className="label">Age:</span>
          <span>{profile.age}</span>
        </div>

        <div className="detail-row">
          <span className="label">Registered on:</span>
          <span>{new Date(profile.createdAt).toLocaleDateString("en-US")}</span>
        </div>

        {profile.bio && (
          <div className="detail-row">
            <span className="label">Bio:</span>
            <span>{profile.bio}</span>
          </div>
        )}
      </div>
    </div>
  );
}
