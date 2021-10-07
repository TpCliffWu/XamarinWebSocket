package gov.sample.socketandroid;

import android.os.Bundle;
import android.os.Handler;
import android.text.TextUtils;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import androidx.annotation.NonNull;
import androidx.fragment.app.Fragment;
import androidx.navigation.fragment.NavHostFragment;

import org.json.JSONObject;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.OutputStreamWriter;
import java.net.InetAddress;
import java.net.Socket;

import gov.sample.socketandroid.databinding.FragmentThirdBinding;

public class ThirdFragment extends Fragment {

    private FragmentThirdBinding binding;
    // 伺服器通訊埠
    private static final int ServerPort = 9999;

    // 客戶端連線
    Socket clientSocket;

    private BufferedWriter bw;  //取得網路輸出串流
    private BufferedReader br;  //取得網路輸入串流

    String s;

    public static Handler mHandler = new Handler();

    // 伺服器傳遞與接收資料的json
    private JSONObject jsonWrite, jsonRead;

    @Override
    public View onCreateView(
            LayoutInflater inflater, ViewGroup container,
            Bundle savedInstanceState
    ) {

        binding = FragmentThirdBinding.inflate(inflater, container, false);
        return binding.getRoot();

    }

    public void onViewCreated(@NonNull View view, Bundle savedInstanceState) {
        super.onViewCreated(view, savedInstanceState);

        // 預設值
        binding.editServerip.setText("192.168.100.147");

        binding.buttonLink.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {

                ConnectToTcpServer();
//                NavHostFragment.findNavController(ThirdFragment.this)
//                        .navigate(R.id.action_ThirdFragment_to_FirstFragment);
            }
        });
    }

    // 連線到TCP 伺服器
    private void ConnectToTcpServer() {

        String tag = "#TCP 連線:";
        // server端的IP
        InetAddress serverIp;
        try {
            String ip = binding.editServerip.getText().toString();

            // 沒有輸入ip
            if (TextUtils.isEmpty(ip)) {
                Log.i(tag, "沒有輸入ip");
                return;
            }

            serverIp = InetAddress.getByName(ip);

            // 建立連線 指定ip 和 port
            clientSocket = new Socket(serverIp, ServerPort);

            //取得網路輸出串流
            bw = new BufferedWriter(new OutputStreamWriter(clientSocket.getOutputStream()));
            //取得網路輸入串流
            br = new BufferedReader(new InputStreamReader(clientSocket.getInputStream()));


            // 當連線後
            while (clientSocket.isConnected()) {
                // 取得網路訊息
                s = br.readLine();

                // 如果不是空訊息則
                if (s != null) {
                    // 顯示新的訊息
                    mHandler.post(updateText);
                }

            }


        } catch (Exception ex) {
            ex.printStackTrace();
        }
    }

    // 更新訊息
    private Runnable updateText = new Runnable() {
        public void run() {
            // 加入新訊息並換行
            binding.textviewThird.append(s + "\n");
        }
    };

    @Override
    public void onDestroyView() {
        super.onDestroyView();

        // 中斷連線
        try {
            if (clientSocket.isConnected()) {
                jsonWrite = new JSONObject();
                jsonWrite.put("action", "離線");
                bw.write(jsonWrite + "\n");
                //立即發送
                bw.flush();
                bw.close();
                br.close();
                clientSocket.close();
            }
        } catch (Exception ex) {
            ex.printStackTrace();
        }

        binding = null;
    }
}