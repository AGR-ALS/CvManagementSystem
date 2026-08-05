import {useCallback, useEffect, useState} from "react";
import {message} from "antd";
import {useTranslation} from "react-i18next";
import {UserProfile} from "../models/UserProfile";
import {CreateSalesforceAccountRequest} from "../models/CreateSalesforceAccountRequest";
import {CreateSalesforceAccount, GetSalesforceAccountStatus} from "../Services/SalesforceService";
import {SalesforceFormValues} from "../Components/SalesforceModal/SalesforceModal";

export function useSalesforce(userId: string, enabled: boolean, user: UserProfile) {
    const {t} = useTranslation();
    const [registered, setRegistered] = useState<boolean | null>(null);
    const [open, setOpen] = useState(false);
    const [submitting, setSubmitting] = useState(false);

    useEffect(() => {
        let cancelled = false;
        if (!enabled || !userId) return;

        setRegistered(null);
        GetSalesforceAccountStatus(userId)
            .then(status => {
                if (!cancelled) setRegistered(status);
            })
            .catch(() => {
                if (!cancelled) setRegistered(null);
            });

        return () => {
            cancelled = true;
        };
    }, [enabled, userId]);

    const openModal = useCallback(() => setOpen(true), []);

    const close = useCallback(() => setOpen(false), []);

    const submit = useCallback(async (values: SalesforceFormValues) => {
        setSubmitting(true);
        try {
            const request: CreateSalesforceAccountRequest = {
                userId,
                accountName: values.accountName,
                accountPhoneNumber: values.accountPhoneNumber,
                accountWebsite: values.accountWebsite,
                contactFirstName: user.profileData?.firstName ?? "",
                contactLastName: user.profileData?.lastName ?? "",
                contactEmail: user.email ?? "",
                contactPhoneNumber: user.profileData?.phoneNumber ?? "",
                contactTitle: values.contactTitle,
            };
            await CreateSalesforceAccount(request);
            const status = await GetSalesforceAccountStatus(userId);
            setRegistered(status);
            setOpen(false);
            message.success(t("messages.salesforceLinked"));
        } catch {
            message.error(t("messages.salesforceLinkError"));
        } finally {
            setSubmitting(false);
        }
    }, [userId, user, t]);

    return {
        registered,
        open,
        submitting,
        openModal,
        close,
        submit,
    };
}
