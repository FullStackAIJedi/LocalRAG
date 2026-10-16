interface AvatarCircleProps {
  imageUrl?: string;
  fallbackText?: string;
}

export default function AvatarCircle({
  imageUrl,
  fallbackText,
}: AvatarCircleProps) {
  return (
    <div className="avatar-circle">
      {imageUrl ? (
        <img src={imageUrl} alt="Avatar" />
      ) : (
        <span>{fallbackText?.charAt(0).toUpperCase() || "?"}</span>
      )}
    </div>
  );
}
