import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import * as signalR from '@microsoft/signalr';
import { AuthService } from '../Auth/AuthService';

export interface ChatMessage {
  senderId: number;
  senderName: string;
  message: string;
  timestamp: Date;
  isStaff: boolean;
}

export interface LichHenUpdate {
  maBenhNhan: number;
  status: string;
  message: string;
}

@Injectable({
  providedIn: 'root'
})
export class ChatService {
  private hubConnection: signalR.HubConnection;
  private messagesSubject = new BehaviorSubject<ChatMessage[]>([]);
  private notificationSubject = new BehaviorSubject<string>('');
  private activePatientsSubject = new BehaviorSubject<number[]>([]);
  private chatPatientsSubject = new BehaviorSubject<number[]>([]);
  private connectionStateSubject = new BehaviorSubject<boolean>(false);
  private lichHenUpdateSubject = new BehaviorSubject<LichHenUpdate | null>(null); // Thêm để xử lý thông báo lịch hẹn

  public messages$ = this.messagesSubject.asObservable();
  public notification$ = this.notificationSubject.asObservable();
  public activePatients$ = this.activePatientsSubject.asObservable();
  public chatPatients$ = this.chatPatientsSubject.asObservable();
  public connectionState$ = this.connectionStateSubject.asObservable();
  public lichHenUpdate$ = this.lichHenUpdateSubject.asObservable(); // Observable cho thông báo lịch hẹn

  constructor(private authService: AuthService) {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5291/chatHub', {
        accessTokenFactory: () => {
          const token = this.authService.getToken();
          if (!token) {
            throw new Error('No token found');
          }
          return token;
        }
      })
      .configureLogging(signalR.LogLevel.Information)
      .build();

    this.startConnection();
  }

  private async startConnection() {
    try {
      await this.hubConnection.start();
      console.log('Connected to SignalR');
      this.connectionStateSubject.next(true);

      this.hubConnection.on('ReceiveMessage', (message: ChatMessage) => {
        console.log('Received message:', message);
        const currentMessages = this.messagesSubject.value;
        this.messagesSubject.next([...currentMessages, message]);
        this.updateChatPatients(message.senderId);
      });

      this.hubConnection.on('StaffJoined', (notification: string) => {
        console.log('Staff joined:', notification);
        this.notificationSubject.next(notification);
      });

      this.hubConnection.on('LoadChatHistory', (messages: ChatMessage[]) => {
        console.log('Loaded chat history:', messages);
        this.messagesSubject.next(messages);
        const chatPatientIds = [...new Set(messages.map(msg => msg.senderId))];
        this.updateChatPatients(chatPatientIds);
      });

      this.hubConnection.on('UpdateActivePatients', (activePatientIds: number[]) => {
        this.activePatientsSubject.next(activePatientIds);
      });

      this.hubConnection.on('LoadChatPatients', (chatPatientIds: number[]) => {
        this.chatPatientsSubject.next(chatPatientIds);
      });

      this.hubConnection.on('Error', (error: string) => {
        console.error('SignalR Error:', error);
      });

      // Xử lý thông báo lịch hẹn từ backend
      this.hubConnection.on('ReceiveLichHenUpdate', (update: LichHenUpdate) => {
        console.log('Received LichHenUpdate:', update);
        this.lichHenUpdateSubject.next(update);
      });
    } catch (err) {
      console.error('Error connecting to SignalR:', err);
      this.connectionStateSubject.next(false);
      setTimeout(() => this.startConnection(), 5000);
    }
  }

  private updateChatPatients(senderId: number | number[]) {
    const currentChatPatients = this.chatPatientsSubject.value;
    if (Array.isArray(senderId)) {
      const newChatPatients = [...new Set([...currentChatPatients, ...senderId])];
      this.chatPatientsSubject.next(newChatPatients);
    } else {
      if (!currentChatPatients.includes(senderId)) {
        this.chatPatientsSubject.next([...currentChatPatients, senderId]);
      }
    }
  }

  public async stopConnection() {
    await this.hubConnection.stop();
    this.connectionStateSubject.next(false);
    console.log('Disconnected from SignalR');
  }

  public async sendMessageToStaff(maTaiKhoan: number, message: string) {
    try {
      await this.hubConnection.invoke('SendMessageToStaff', maTaiKhoan, message);
    } catch (err) {
      console.error('Error sending message to staff:', err);
      throw err;
    }
  }

  public async joinChat(maNhanVien: number, maBenhNhan: number) {
    try {
      await this.hubConnection.invoke('JoinChat', maNhanVien, maBenhNhan);
    } catch (err) {
      console.error('Error joining chat:', err);
    }
  }

  public async sendMessageToPatient(maNhanVien: number, maBenhNhan: number, message: string) {
    try {
      await this.hubConnection.invoke('SendMessageToPatient', maNhanVien, maBenhNhan, message);
    } catch (err) {
      console.error('Error sending message to patient:', err);
    }
  }
}