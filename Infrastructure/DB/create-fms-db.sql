------------------------------------------------
-- Enable pgcrypto only if not installed (for gen_random_uuid)
------------------------------------------------
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_extension WHERE extname = 'pgcrypto') THEN
        CREATE EXTENSION pgcrypto;
    END IF;
END$$;


------------------------------------------------
-- Lookup table: user_types
-- (matches your C# enum: Admin=0, Costumer=1)
------------------------------------------------
CREATE TABLE IF NOT EXISTS user_types (
    id   INTEGER PRIMARY KEY,
    name VARCHAR(50) NOT NULL UNIQUE
);

-- Seed enum values (safe to run multiple times)
INSERT INTO user_types (id, name) VALUES
    (0, 'Admin'),
    (1, 'Costumer')
ON CONFLICT (id) DO UPDATE
SET name = EXCLUDED.name;

------------------------------------------------
-- Users table
------------------------------------------------
CREATE TABLE IF NOT EXISTS users (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),

    user_type INTEGER NOT NULL,   -- stores enum int (0/1/...)

    email VARCHAR(255) NOT NULL,
    first_name VARCHAR(100) NOT NULL,
    last_name VARCHAR(100) NOT NULL,

    created_date TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    modified_date TIMESTAMPTZ NOT NULL DEFAULT NOW(),

    -- Enforce validity: must exist in user_types
    CONSTRAINT fk_users_user_type
        FOREIGN KEY (user_type)
        REFERENCES user_types(id)
        ON UPDATE RESTRICT
        ON DELETE RESTRICT
);


------------------------------------------------
-- Unique index on email
------------------------------------------------
CREATE UNIQUE INDEX IF NOT EXISTS idx_users_email_unique
    ON users(email);

-- (Optional) index for queries filtering by user_type
CREATE INDEX IF NOT EXISTS idx_users_user_type
    ON users(user_type);