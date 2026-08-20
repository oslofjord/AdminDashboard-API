import { Router } from 'express';
import eventsRouter from './events';

const router = Router();

router.use('/events', eventsRouter);

router.get('/health', (req, res) => {
  res.json({ 
    status: 'ok', 
    service: 'AdminDashboard-API',
    port: 5200,
    timestamp: new Date().toISOString()
  });
});

export default router;
