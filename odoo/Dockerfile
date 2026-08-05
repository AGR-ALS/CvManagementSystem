FROM odoo:19

USER root

COPY ./addons/position_import_addon /mnt/extra-addons/position_import_addon

USER odoo

EXPOSE 8069

CMD ["odoo", "--addons-path", "/usr/lib/python3/dist-packages/odoo/addons,/mnt/extra-addons"]