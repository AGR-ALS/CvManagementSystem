export interface DiscussionMessage {
    id: string;
    text: string;
    userId: string;
    discussionId: string;
    sentAt: string;
}

export interface Discussion {
    id: string;
    positionId: string;
    messages: DiscussionMessage[];
}