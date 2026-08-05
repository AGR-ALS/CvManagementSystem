import {Priority} from "./Priority";

export interface CreateSupportTicketRequest {
    summary: string;
    positionId: string | null;
    pageLink: string;
    priority: Priority;
}
