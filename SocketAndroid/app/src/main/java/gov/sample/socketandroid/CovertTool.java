package gov.sample.socketandroid;

public class CovertTool {

    //byte轉換，輸出String，避免java轉換為string或int時出現負號，導致判斷錯誤
    public static long byteTointstring(byte b){
        return(Long.parseLong( byteToString(b), 16));
    }

    public static String byteToString(byte b) {
        byte high, low;
        byte maskHigh = (byte)0xf0;
        byte maskLow = 0x0f;

        high = (byte)((b & maskHigh) >> 4);
        low = (byte)(b & maskLow);

        StringBuffer buf = new StringBuffer();
        buf.append(findHex(high));
        buf.append(findHex(low));

        return buf.toString();
    }

    private static char findHex(byte b) {
        int t = new Byte(b).intValue();
        t = t < 0 ? t + 16 : t;

        if ((0 <= t) &&(t <= 9)) {
            return (char)(t + '0');
        }

        return (char)(t-10+'A');
    }
}
