CREATE TABLE IF NOT EXISTS Users (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    Email VARCHAR(320) NOT NULL,
    GoogleId VARCHAR(255) NOT NULL,
    Picture TEXT NULL,
    Names VARCHAR(255) NULL,
    IsActive BOOLEAN NOT NULL DEFAULT true,
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    LastLoginAt TIMESTAMP NULL
);

CREATE UNIQUE INDEX IF NOT EXISTS IX_Users_Email ON Users(Email);
CREATE UNIQUE INDEX IF NOT EXISTS IX_Users_GoogleId ON Users(GoogleId);

INSERT INTO Users (Email, GoogleId, Names, CreatedAt)
VALUES (
    'bot@user.ia',
    'google_fake_123456789',
    'T-800',
    CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS ChatRooms (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    Name TEXT NOT NULL,
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

INSERT INTO ChatRooms 
    (Name, CreatedAt)
VALUES (
    'Sala de Prueba', 
    CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS Messages (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    Content TEXT NOT NULL,
    SentAt TIMESTAMP NOT NULL,
    UserId UUID NOT NULL REFERENCES Users(Id),
    ChatRoomId UUID NOT NULL REFERENCES ChatRooms(Id)
);

INSERT INTO Messages 
    (Content, SentAt, UserId, ChatRoomId)
VALUES (
    '¡Hola desde REST!', 
    CURRENT_TIMESTAMP, 
    '849e20e0-c5b2-407d-97f3-4e26f7198a37', 
    '1fb61def-542c-4dfc-9c65-fc772a89fa53'
);

SELECT 
	a.id AS UserId,
	b.id AS ChatRommId
FROM Users AS a, ChatRooms AS b;

SELECT * FROM Users;
SELECT * FROM ChatRooms;
SELECT * FROM Messages;

DROP TABLE IF EXISTS Messages;
DROP TABLE IF EXISTS ChatRooms;
DROP TABLE IF EXISTS Users;
