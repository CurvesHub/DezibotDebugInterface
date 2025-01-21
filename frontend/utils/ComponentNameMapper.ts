export function getCompName(id: string): string {
    const { t } = useI18n() 
    switch(id) {
        case "DISPLAY": return t("comp_name_display")
        case "LIGHT_DETECT": return t("comp_name_light_detect")
        case "COLOR_DETECT": return t("comp_name_color_detect")
        case "MULTI_COLOR_LIGHT": return t("comp_name_multi_color_light")
        case "MOTOR": return t("comp_name_motor")
        case "MAIN": return t("comp_name_main")
        case "COMMUNICATION": return t("comp_name_communication")
        case "RGB_SENSOR": return t("comp_name_rgb_sensor")
        case "INFRARED": return t("comp_name_infrared")
        case "MOTION": return t("comp_name_motion_detect")
        default: return id
    }
}
