import { useState } from "react";
import type { FormEvent } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useRegister } from "../hooks/useAuthMutations";
import type { RegisterRequest } from "../api/types";
import AvatarCircle from "../components/AvatarCircle";

export default function RegisterPage() {
  const navigate = useNavigate();
  const [error, setError] = useState("");
  const [form, setForm] = useState<RegisterRequest>({
    nickname: "",
    email: "",
    password: "",
    avatarUrl: "",
    age: 0,
    firstName: "",
    lastName: "",
    bio: "",
    country: "",
    city: "",
  });

  const registerMutation = useRegister();

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setError("");

    try {
      await registerMutation.mutateAsync(form);
      navigate("/login");
    } catch (err: any) {
      setError(err.response?.data?.error || "Registration failed.");
    }
  };

  const update = (field: keyof RegisterRequest, value: any) => {
    setForm((prev) => ({ ...prev, [field]: value }));
  };

  return (
    <div className="auth-container-register">
      <div className="auth-card-register">
        <h1>Register</h1>

        <AvatarCircle
          imageUrl={form.avatarUrl || undefined}
          fallbackText={form.nickname}
        />

        <form onSubmit={handleSubmit}>
          {/* Single column layout */}
          <div className="form-group">
            <label>Nickname</label>
            <input
              value={form.nickname}
              onChange={(e) => update("nickname", e.target.value)}
              required
            />
          </div>

          <div className="form-group">
            <label>Email</label>
            <input
              type="email"
              value={form.email}
              onChange={(e) => update("email", e.target.value)}
              required
            />
          </div>

          <div className="form-group">
            <label>Password</label>
            <input
              type="password"
              value={form.password}
              onChange={(e) => update("password", e.target.value)}
              required
            />
          </div>

          <div className="form-group">
            <label>Avatar URL</label>
            <input
              value={form.avatarUrl}
              onChange={(e) => update("avatarUrl", e.target.value)}
              placeholder="https://example.com/avatar.jpg"
            />
          </div>

          <div className="form-group">
            <label>First Name</label>
            <input
              value={form.firstName}
              onChange={(e) => update("firstName", e.target.value)}
            />
          </div>

          <div className="form-group">
            <label>Last Name</label>
            <input
              value={form.lastName}
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
              value={form.country}
              onChange={(e) => update("country", e.target.value)}
            />
          </div>

          <div className="form-group">
            <label>City</label>
            <input
              value={form.city}
              onChange={(e) => update("city", e.target.value)}
            />
          </div>

          <div className="form-group">
            <label>Bio</label>
            <textarea
              value={form.bio}
              onChange={(e) => update("bio", e.target.value)}
              rows={3}
            />
          </div>

          {error && <div className="error">{error}</div>}

          <button type="submit" disabled={registerMutation.isPending}>
            {registerMutation.isPending ? "Registering..." : "Register"}
          </button>
        </form>

        <p className="auth-link">
          Already have an account? <Link to="/login">Log in</Link>
        </p>
      </div>
    </div>
  );
}
