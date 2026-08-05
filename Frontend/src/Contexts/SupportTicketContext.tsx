import {createContext, ReactNode, useContext, useMemo} from "react";
import {useSupportTicket} from "../Hooks/useSupportTicket";
import SupportTicketModal from "../Components/SupportTicketModal/SupportTicketModal";
import SupportTicketFab from "../Components/SupportTicketFab/SupportTicketFab";

interface SupportTicketContextType {
    openSupportTicket: (positionId?: string | null) => void;
}

const SupportTicketContext = createContext<SupportTicketContextType | undefined>(undefined);

export function useSupportTicketContext() {
    const context = useContext(SupportTicketContext);
    if (!context) {
        throw new Error("SupportTicketContext must be defined");
    }
    return context;
}

interface Props {
    children: ReactNode;
}

export function SupportTicketProvider({children}: Props) {
    const {
        open,
        submitting,
        openSupportTicket,
        closeSupportTicket,
        submitSupportTicket,
    } = useSupportTicket();

    const value = useMemo<SupportTicketContextType>(() => ({openSupportTicket}), [openSupportTicket]);

    return (
        <SupportTicketContext value={value}>
            {children}
            <SupportTicketModal
                open={open}
                submitting={submitting}
                onCancel={closeSupportTicket}
                onSubmit={submitSupportTicket}
            />
            <SupportTicketFab
                onClick={() => openSupportTicket()}
            />
        </SupportTicketContext>
    );
}
