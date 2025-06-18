package co.xbab.maestral

interface Platform {
    val name: String
}

expect fun getPlatform(): Platform