import {TableColumnsType, TableProps} from "antd";
import {useTranslation} from "react-i18next";
import GenericTable from "../CommonTable/CommonTable";
import {CvBasicModel} from "../../models/CvBasicModel";

interface Props {
    data: CvBasicModel[];
    handleOpen: (cv: CvBasicModel) => void;
    rowSelection?: TableProps<CvBasicModel>['rowSelection'];
}

export default function CvListTable({data, handleOpen, rowSelection}: Props) {
    const {t} = useTranslation();

    const columns: TableColumnsType<CvBasicModel> = [
        {
            title: t("cvList.user"),
            dataIndex: "username",
            key: "username",
            width: "35%",
        },
        {
            title: t("cvList.position"),
            dataIndex: "positionTitle",
            key: "positionTitle",
            width: "45%",
        },
        {
            title: t("cvList.likes"),
            dataIndex: "likes",
            key: "likes",
            width: "20%",
        },
    ];

    return (
        <GenericTable<CvBasicModel>
            dataSource={data}
            columns={columns}
            rowKey="id"
            onRowClick={handleOpen}
            rowSelection={rowSelection}
        />
    );
}
