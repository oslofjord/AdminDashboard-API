import app from './app';
import { config } from './config';

const server = app.listen(config.port, () => {
  console.log(`🚀 AdminDashboard-API running on port ${config.port}`);
  console.log(`📍 Environment: ${config.nodeEnv}`);
  console.log(`🔗 Central API: ${config.centralApiUrl}`);
  console.log(`✅ Health check: http://localhost:${config.port}/api/health`);
});

process.on('SIGTERM', () => {
  console.log('SIGTERM signal received: closing HTTP server');
  server.close(() => {
    console.log('HTTP server closed');
  });
});
