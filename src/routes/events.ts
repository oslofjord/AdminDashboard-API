import { Router } from 'express';
import { eventsController } from '../controllers/eventsController';

const router = Router();

router.get('/', eventsController.getAll);
router.get('/:id', eventsController.getById);
router.post('/', eventsController.create);
router.put('/:id', eventsController.update);
router.delete('/:id', eventsController.delete);
router.post('/:id/enrich', eventsController.enrich);

export default router;
