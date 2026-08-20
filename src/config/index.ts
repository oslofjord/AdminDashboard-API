import dotenv from 'dotenv';

dotenv.config();

export const config = {
  port: process.env.PORT || 5200,
  nodeEnv: process.env.NODE_ENV || 'development',
  centralApiUrl: process.env.CENTRAL_API_URL || 'http://localhost:5100',
  centralApiKey: process.env.CENTRAL_API_KEY || '',
  allowedOrigins: process.env.ALLOWED_ORIGINS?.split(',') || ['http://localhost:3002'],
  jwtSecret: process.env.JWT_SECRET || 'your-secret-key-change-in-production',
};
