import { Link, Outlet, useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { useLogout } from "../hooks/useAuthMutations";

export default function Layout() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const logoutMutation = useLogout();

  const handleLogout = async () => {
    try {
      await logoutMutation.mutateAsync();
    } catch {}
    logout();
    navigate("/login");
  };

  return (
    <div className="layout">
      <header className="header">
        <div className="header-content">
          <div className="header-left">
            <h1 className="logo">
              Local RAG <span className="logo-sub">Document Q&A</span>
            </h1>
            <nav className="nav">
              <Link to="/search">Search</Link>
              <Link to="/profile">Profile</Link>
            </nav>
          </div>

          <div className="user-info">
            {user && (
              <>
                {user.avatarUrl && (
                  <img
                    src={user.avatarUrl}
                    alt="Avatar"
                    className="avatar-small"
                  />
                )}
                <span>{user.nickname}</span>
              </>
            )}
            <button
              onClick={handleLogout}
              className="btn-logout"
              disabled={logoutMutation.isPending}
            >
              Sign out
            </button>
          </div>
        </div>
      </header>
      <main className="main-content">
        <Outlet />
      </main>
    </div>
  );
}
