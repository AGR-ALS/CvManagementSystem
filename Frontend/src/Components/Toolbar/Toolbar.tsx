import React from "react";
import {useTranslation} from "react-i18next";
import "./Toolbar.css";

interface Props {
    selectedRowKeys: React.Key[];
    onCreate?: () => void;
    onDelete?: (ids: React.Key[]) => void;
    onUnlock?: (ids: React.Key[]) => void;
    onBlock?: (ids: React.Key[]) => void;
}

export function Toolbar({selectedRowKeys, onCreate, onDelete, onUnlock, onBlock}: Props) {
    const {t} = useTranslation();
    return (
        <div className="toolbar__container">
            {onCreate && (
                <button
                    type='button'
                    className="btn btn-primary toolbar__btn"
                    onClick={onCreate}
                    title={t("toolbar.create")}
                >
                    <i className="bi bi-plus-lg"></i>
                </button>
            )}
            {onDelete && (
                <button
                    type='button'
                    className="btn btn-danger toolbar__btn"
                    onClick={() => onDelete(selectedRowKeys)}
                    title={t("toolbar.delete")}
                >
                    <i className="bi bi-trash"></i>
                </button>
            )}
            {onBlock && (
                <button
                    type='button'
                    className="btn btn-warning toolbar__btn"
                    onClick={() => onBlock(selectedRowKeys)}
                    title={t("toolbar.block")}
                >
                    <i className="bi bi-lock"></i>
                </button>
            )}
            {onUnlock && (
                <button
                    type='button'
                    className="btn btn-success toolbar__btn"
                    onClick={() => onUnlock(selectedRowKeys)}
                    title={t("toolbar.unlock")}
                >
                    <i className="bi bi-unlock"></i>
                </button>
            )}
        </div>
    )
}