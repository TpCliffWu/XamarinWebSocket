package gov.sample.socketandroid;

import android.content.Context;
import android.net.wifi.WifiManager;
import android.os.AsyncTask;
import android.os.Bundle;
import android.os.Handler;
import android.text.format.Formatter;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.fragment.app.Fragment;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.OutputStreamWriter;
import java.io.PrintWriter;
import java.net.Inet4Address;
import java.net.InetAddress;
import java.net.ServerSocket;
import java.net.Socket;
import java.net.UnknownHostException;
import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

import gov.sample.socketandroid.databinding.FragmentSecondBinding;

public class SecondFragment extends Fragment {

    private FragmentSecondBinding binding;

    // client 對象清單
    private List<Socket> ClientList = new ArrayList<Socket>();

    // 伺服器通訊埠
    private static final int ServerPort = 9999;

    // socket 伺服器
    private volatile ServerSocket serverSocket = null;

    // 執行緒
    private ExecutorService mExecutorService = null;

    // 執行緒狀態
    private volatile boolean flag = true;

    private Handler handler = new Handler();

    // 接收client傳送過來的訊息
    String response;

    String SystemMsg;

    @Override
    public View onCreateView(
            LayoutInflater inflater, ViewGroup container,
            Bundle savedInstanceState
    ) {

        OnListen = false;
        binding = FragmentSecondBinding.inflate(inflater, container, false);
        return binding.getRoot();
    }

    public boolean OnListen;

    public void onViewCreated(@NonNull View view, Bundle savedInstanceState) {
        super.onViewCreated(view, savedInstanceState);

        binding.buttonListen.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                if (OnListen) {

                    StopListen();
                } else {

                    StartListen();
                }

//                NavHostFragment.findNavController(SecondFragment.this)
//                        .navigate(R.id.action_SecondFragment_to_FirstFragment);
            }
        });
    }

    // 開啟Server
    private void StartListen() {
        OnListen = true;
        binding.buttonListen.setText("Stop Listen");
        flag = true;


        // 顯示ip 位置
        try {
            // IP 位置
            // [TODO] 支援 IPV6
            Context context = requireContext().getApplicationContext();
            WifiManager wm = (WifiManager) context.getSystemService(Context.WIFI_SERVICE);
            String ip = Formatter.formatIpAddress(wm.getConnectionInfo().getIpAddress());
            AddText(ip);

        } catch (Exception e) {
            e.printStackTrace();
            return;
        }

        AddText("開啟伺服器");


        try {
            serverSocket = new ServerSocket(ServerPort);

          BufferedReader bufferedReader;
            while (!serverSocket.isClosed()) {
                // 呼叫等待接受客戶端連接
                Socket socket = serverSocket.accept();

                bufferedReader = new BufferedReader(new InputStreamReader(socket.getInputStream()));

                Log.i("#Receieve Message:", bufferedReader.readLine());
            }


        } catch (IOException e) {
            e.printStackTrace();
        }
        System.out.println("Server is start.");
    }

    // 關閉Server
    private void StopListen() {

        OnListen = false;
        binding.buttonListen.setText("Start Listen");
        flag = false;

        AddText("關閉伺服器");
        // 關閉連線
        try {
            serverSocket.close();
        } catch (Exception ex) {
            ex.printStackTrace();
        }

        for (int c = 0; c < ClientList.size(); c++) {
            Socket s = ClientList.get(c);
            try {
                s.close();
            } catch (IOException e) {
                e.printStackTrace();
            }
        }
        AddText("伺服器已關閉");
    }

    class UpdateText implements Runnable {

        String msg;
        public  UpdateText(String msg){
            this.msg =msg;
        }

        @Override
        public void run() {
            binding.textviewSecond.append(msg + "\n");
        }
    }



    @Override
    public void onDestroyView() {
        super.onDestroyView();
        binding = null;
    }

    public void AddText(String text){

        handler.post(new Runnable() {
            @Override
            public void run() {
                binding.textviewSecond.append(text + "\n");
            }
        });
    }


//
//    class ServerRecieveMsg implements Runnable {
//
//        private Socket socket;
//        private BufferedReader in = null;
//        private String msg = "";
//        boolean status = true;
//
//        // 輸入流物件
//        InputStream is;
//
//        // 輸入流讀取器物件
//        InputStreamReader isr;
//        BufferedReader br;
//
//        public ServerRecieveMsg(Socket socket) {
//
//            this.socket = socket;
//            try {
//                in = new BufferedReader(new InputStreamReader(socket.getInputStream(), "UTF-8"));
//
//            } catch (IOException e) {
//                e.printStackTrace();
//            }
//
//        }
//
//        @Override
//        public void run() {
//            while (status) {
//                try {
//
//                    // 步驟1：建立輸入流物件InputStream
//                    is = socket.getInputStream();
//
//                    // 步驟2：建立輸入流讀取器物件 並傳入輸入流物件
//                    // 該物件作用：獲取伺服器返回的資料
//                    isr = new InputStreamReader(is);
//                    br = new BufferedReader(isr);
//
//                    // 步驟4:通知主執行緒,將接收的訊息顯示到介面
//                    // 步驟3：通過輸入流讀取器物件 接收伺服器傳送過來的資料
//                    response = br.readLine();
//
//                } catch (IOException e) {
//                    System.out.println("close");
//                    status = false;
//                    e.printStackTrace();
//                }
//            }
//        }
//
//        public void sendmsg(String msg) {
//            System.out.println(msg);
//            PrintWriter pout = null;
//            try {
//                pout = new PrintWriter(new BufferedWriter(
//                        new OutputStreamWriter(socket.getOutputStream())), true);
//                pout.println(msg);
//            } catch (IOException e) {
//                e.printStackTrace();
//            }
//        }
//    }


}
