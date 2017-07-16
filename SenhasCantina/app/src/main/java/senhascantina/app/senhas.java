package senhascantina.app;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;
import android.widget.Button;
import android.widget.Toast;

public class senhas extends Activity {
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        //Remove title bar
        this.requestWindowFeature(Window.FEATURE_NO_TITLE);

        //Remove notification bar
        this.getWindow().setFlags(WindowManager.LayoutParams.FLAG_FULLSCREEN, WindowManager.LayoutParams.FLAG_FULLSCREEN);

        setContentView(R.layout.activity_senhas);
        tickets();
    }

    private void tickets() {
        Button btnBuy = (Button) findViewById(R.id.btnComprar);
        btnBuy.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                startActivity(new Intent(senhas.this, comprarSenhas.class ));
            }
        });

        Button btnSee = (Button) findViewById(R.id.btnVer);
        btnSee.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                startActivity(new Intent(senhas.this, verSenhas.class));
            }
        });

    }

    @Override
    public void onBackPressed() {
        startActivity(new Intent(senhas.this, MainActivity.class ));
    }
}