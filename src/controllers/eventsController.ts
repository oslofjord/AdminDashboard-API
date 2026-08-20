import { Request, Response, NextFunction } from 'express';
import { centralApiService } from '../services/centralApiService';
import { ApiError } from '../middleware/errorHandler';

export const eventsController = {
  async getAll(req: Request, res: Response, next: NextFunction) {
    try {
      const events = await centralApiService.getEvents(req.query);
      res.json(events);
    } catch (error) {
      next(new ApiError(500, 'Failed to fetch events from Central API'));
    }
  },

  async getById(req: Request, res: Response, next: NextFunction) {
    try {
      const event = await centralApiService.getEventById(req.params.id);
      res.json(event);
    } catch (error) {
      next(new ApiError(404, 'Event not found'));
    }
  },

  async create(req: Request, res: Response, next: NextFunction) {
    try {
      const event = await centralApiService.createEvent(req.body);
      res.status(201).json(event);
    } catch (error) {
      next(new ApiError(400, 'Failed to create event'));
    }
  },

  async update(req: Request, res: Response, next: NextFunction) {
    try {
      const event = await centralApiService.updateEvent(req.params.id, req.body);
      res.json(event);
    } catch (error) {
      next(new ApiError(400, 'Failed to update event'));
    }
  },

  async delete(req: Request, res: Response, next: NextFunction) {
    try {
      await centralApiService.deleteEvent(req.params.id);
      res.status(204).send();
    } catch (error) {
      next(new ApiError(400, 'Failed to delete event'));
    }
  },

  async enrich(req: Request, res: Response, next: NextFunction) {
    try {
      const enrichedEvent = await centralApiService.enrichEvent(req.params.id, req.body);
      res.json(enrichedEvent);
    } catch (error) {
      next(new ApiError(400, 'Failed to enrich event'));
    }
  },
};
