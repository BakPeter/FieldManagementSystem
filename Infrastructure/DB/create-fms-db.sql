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
-- users
------------------------------------------------

-- Lookup table: user_types
-- (matches enum: Admin=0, Costumer=1)
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

-- Users table
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


-- Unique index on email
CREATE UNIQUE INDEX IF NOT EXISTS idx_users_email_unique
    ON users(email);

-- (Optional) index for queries filtering by user_type
CREATE INDEX IF NOT EXISTS idx_users_user_type
    ON users(user_type);

------------------------------------------------

------------------------------------------------

------------------------------------------------
-- fields
------------------------------------------------

-- Fields table
------------------------------------------------
CREATE TABLE IF NOT EXISTS fields (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(200) NOT NULL,
    description TEXT NOT NULL,
    created_date TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    modified_date TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- Optional: unique name
CREATE UNIQUE INDEX IF NOT EXISTS idx_fields_name_unique ON fields(name);


-- Join table: which users can access which field
CREATE TABLE IF NOT EXISTS field_authorized_users (
    field_id UUID NOT NULL,
    user_id  UUID NOT NULL,

    PRIMARY KEY (field_id, user_id),

    CONSTRAINT fk_fau_field
        FOREIGN KEY (field_id) REFERENCES fields(id)
        ON DELETE CASCADE,

    CONSTRAINT fk_fau_user
        FOREIGN KEY (user_id) REFERENCES users(id)
        ON DELETE RESTRICT
);

CREATE INDEX IF NOT EXISTS idx_fau_user_id ON field_authorized_users(user_id);
CREATE INDEX IF NOT EXISTS idx_fau_field_id ON field_authorized_users(field_id);


