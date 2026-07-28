import DiscussionMessageItem from "./DiscussionMessageItem";
import {DiscussionMessage} from "../../models/Discussion";

interface Props {
    messages: DiscussionMessage[];
    authorNames: Record<string, string>;
}

export default function DiscussionList({ messages, authorNames }: Props) {
    return (
        <div className="d-flex flex-column gap-3">
            {messages.map(msg => (
                <DiscussionMessageItem
                    key={msg.id}
                    message={msg}
                    authorName={authorNames[msg.userId]}
                />
            ))}
        </div>
    )
}
