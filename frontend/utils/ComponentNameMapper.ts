export function getCompName(id: string): string {
  switch(true) {
      case id=="DISPLAY": return "Display"
      case id=="LIGHT_DETECT": return "Light Detection"
      default: return ""
  }
}
