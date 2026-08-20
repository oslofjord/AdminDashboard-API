import axios, { AxiosInstance } from 'axios';
import { config } from '../config';

class CentralApiService {
  private client: AxiosInstance;

  constructor() {
    this.client = axios.create({
      baseURL: config.centralApiUrl,
      headers: {
        'Content-Type': 'application/json',
        ...(config.centralApiKey && { 'Authorization': `Bearer ${config.centralApiKey}` }),
      },
      timeout: 30000,
    });
  }

  async getEvents(params?: any) {
    const response = await this.client.get('/api/events', { params });
    return response.data;
  }

  async getEventById(id: string) {
    const response = await this.client.get(`/api/events/${id}`);
    return response.data;
  }

  async createEvent(data: any) {
    const response = await this.client.post('/api/events', data);
    return response.data;
  }

  async updateEvent(id: string, data: any) {
    const response = await this.client.put(`/api/events/${id}`, data);
    return response.data;
  }

  async deleteEvent(id: string) {
    const response = await this.client.delete(`/api/events/${id}`);
    return response.data;
  }

  async enrichEvent(id: string, enrichmentData: any) {
    const response = await this.client.post(`/api/events/${id}/enrich`, enrichmentData);
    return response.data;
  }
}

export const centralApiService = new CentralApiService();
