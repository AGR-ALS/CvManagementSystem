import {useCallback, useState} from "react";
import {message} from "antd";
import {useTranslation} from "react-i18next";
import {CreateSupportTicketRequest} from "../models/CreateSupportTicketRequest";
import {CreateSupportTicket} from "../Services/SupportTicketService";
import {SupportTicketFormValues} from "../Components/SupportTicketModal/SupportTicketModal";

export function useSupportTicket() {
    const {t} = useTranslation();
    const [open, setOpen] = useState(false);
    const [positionId, setPositionId] = useState<string | null>(null);
    const [submitting, setSubmitting] = useState(false);

    const openSupportTicket = useCallback((positionId?: string | null) => {
        setPositionId(positionId ?? null);
        setOpen(true);
    }, []);

    const closeSupportTicket = useCallback(() => {
        setOpen(false);
    }, []);

    const submitSupportTicket = async (values: SupportTicketFormValues) => {
        const request: CreateSupportTicketRequest = {
            summary: values.summary,
            positionId,
            pageLink: window.location.href,
            priority: values.priority,
        };
        setSubmitting(true);
        try {
            await CreateSupportTicket(request);
            message.success(t("messages.supportTicketSent"));
            setOpen(false);
        } catch {
            message.error(t("messages.supportTicketSendError"));
        } finally {
            setSubmitting(false);
        }
    };

    return {
        open,
        submitting,
        positionId,
        openSupportTicket,
        closeSupportTicket,
        submitSupportTicket,
    };
}
