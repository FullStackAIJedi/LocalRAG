import { Link, Outlet, useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

export default function Layout() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate("/login");
  };

  return (
    <div className="layout">
      <header className="header">
        <div className="header-content">
          <h1 className="logo">RAG app</h1>
          <nav className="nav">
            <Link to="/search">Search</Link>
            <Link to="/profile">Profile</Link>
          </nav>
          <div className="user-info">
            {user && (
              <>
                {user.avatarUrl && (
                  <img
                    src={user.avatarUrl}
                    alt="Аватар"
                    className="avatar-small"
                  />
                )}
                <span>{user.nickname}</span>
              </>
            )}
            <button onClick={handleLogout} className="btn-logout">
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
