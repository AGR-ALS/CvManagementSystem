import DiscussionMessageItem from "./DiscussionMessageItem";
import {DiscussionMessage} from "../../models/Discussion";

interface Props {
    messages: DiscussionMessage[];
    authorNames: Record<string, string>;
}

export default function DiscussionList({messages, authorNames}: Props) {
    const sortedMessages = [...messages].sort((a, b) => new Date(a.sentAt).getTime() - new Date(b.sentAt).getTime());
    return <div className="d-flex flex-column gap-3">{sortedMessages.map(msg => <DiscussionMessageItem key={msg.id}
                                                                                                       message={msg}
                                                                                                       authorName={authorNames[msg.userId]}/>)}</div>;
}
