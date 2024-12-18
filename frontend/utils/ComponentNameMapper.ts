export function getCompName(id: string): string {
  switch(true) {
      case id=="DISPLAY": return "Display"
      case id=="LIGHT_DETECT": return "Light Detection"
      case id=="COLOR_DETECT": return "Color Detection"
      case id=="MULTI_COLOR_LIGHT": return "Multi Color Light"
      case id=="MOTOR": return "Motor"
      case id=="MAIN": return "Main Program"
      default: return id
  }
}
