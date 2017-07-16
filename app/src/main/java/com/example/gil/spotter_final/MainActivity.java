package com.example.gil.spotter_final;

import android.graphics.Color;
import android.os.Bundle;
import android.support.design.widget.FloatingActionButton;
import android.support.design.widget.Snackbar;
import android.util.Log;
import android.view.View;
import android.support.design.widget.NavigationView;
import android.support.v4.view.GravityCompat;
import android.support.v4.widget.DrawerLayout;
import android.support.v7.app.ActionBarDrawerToggle;
import android.support.v7.app.AppCompatActivity;
import android.support.v7.widget.Toolbar;
import android.view.Menu;
import android.view.MenuItem;

import com.github.mikephil.charting.charts.BarChart;
import com.github.mikephil.charting.data.BarData;
import com.github.mikephil.charting.data.BarDataSet;
import com.github.mikephil.charting.data.BarEntry;
import com.github.mikephil.charting.utils.ColorTemplate;
import com.google.firebase.database.DataSnapshot;
import com.google.firebase.database.DatabaseError;
import com.google.firebase.database.DatabaseReference;
import com.google.firebase.database.FirebaseDatabase;
import com.google.firebase.database.ValueEventListener;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.Iterator;
import java.util.List;
import java.util.Map;
import java.util.Set;

public class MainActivity extends AppCompatActivity
        implements NavigationView.OnNavigationItemSelectedListener {

    private static final String TAG = "";

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        Toolbar toolbar = (Toolbar) findViewById(R.id.toolbar);
        setSupportActionBar(toolbar);


        DrawerLayout drawer = (DrawerLayout) findViewById(R.id.drawer_layout);
        ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(
                this, drawer, toolbar, R.string.navigation_drawer_open, R.string.navigation_drawer_close);
        drawer.setDrawerListener(toggle);
        toggle.syncState();

        NavigationView navigationView = (NavigationView) findViewById(R.id.nav_view);
        navigationView.setNavigationItemSelectedListener(this);

        FirebaseDatabase database = FirebaseDatabase.getInstance();
        DatabaseReference myRef = database.getReference("objects");

        // Read from the database
        myRef.addValueEventListener(new ValueEventListener() {


            @Override
            public void onDataChange(DataSnapshot dataSnapshot) {
                // This method is called once with the initial value and again
                // whenever data at this location is updated.

                ArrayList<Map<String,String>> list = (ArrayList<Map<String,String>>) dataSnapshot.getValue();
                HashMap<String, Integer > hmap = new HashMap<String, Integer>();
                int count = 1;
                for (Map<String,String> map : list) {
                    if(map!=null) {
                        if(map.get("type").equals("person")) {
                            if(hmap.containsKey(map.get("time").split(":")[0])) {
                                int a = hmap.get(map.get("time").split(":")[0]);
                                hmap.put(map.get("time").toString().split(":")[0], ++a);
                            }
                            else
                                hmap.put(map.get("time").toString().split(":")[0], 1);
                        }
                    }
                }
                BarChart chart = (BarChart) findViewById(R.id.chart);

                BarData data = new BarData(getXAxisValues(), getDataSet(hmap));
                chart.setData(data);
                chart.setDescription("People by hour");
                chart.animateXY(2000, 2000);
                chart.invalidate();
            }

            @Override
            public void onCancelled(DatabaseError error) {
                // Failed to read value
                Log.w(TAG, "Failed to read value.", error.toException());
            }
        });
    }

    public ArrayList<BarDataSet> getDataSet(HashMap<String, Integer> hmap) {
        ArrayList<BarDataSet> dataSets = null;

        ArrayList<BarEntry> valueSet = new ArrayList<>();

        //Introduzir valores no gráfico
        Set set = hmap.entrySet();
        Iterator iterator = set.iterator();

        while(iterator.hasNext()) {
            Map.Entry mentry = (Map.Entry) iterator.next();
            int a = Integer.parseInt(mentry.getKey().toString());
            BarEntry a1 = new BarEntry(Float.parseFloat(mentry.getValue().toString()), a);
            valueSet.add(a1);
        }

        BarDataSet barDataSet1 = new BarDataSet(valueSet, "Amount of people");
        barDataSet1.setColor(Color.rgb(0, 155, 0));

        dataSets = new ArrayList<>();
        dataSets.add(barDataSet1);
        return dataSets;
    }

    private ArrayList<String> getXAxisValues() {
        ArrayList<String> xAxis = new ArrayList<>();
        xAxis.add("00h");
        xAxis.add("01h");
        xAxis.add("02h");
        xAxis.add("03h");
        xAxis.add("04h");
        xAxis.add("05h");
        xAxis.add("06h");
        xAxis.add("07h");
        xAxis.add("08h");
        xAxis.add("09h");
        xAxis.add("10h");
        xAxis.add("11h");
        xAxis.add("12h");
        xAxis.add("13h");
        xAxis.add("14h");
        xAxis.add("15h");
        xAxis.add("16h");
        xAxis.add("17h");
        xAxis.add("18h");
        xAxis.add("19h");
        xAxis.add("20h");
        xAxis.add("21h");
        xAxis.add("22h");
        xAxis.add("23h");

        return xAxis;
    }

    @Override
    public void onBackPressed() {
        DrawerLayout drawer = (DrawerLayout) findViewById(R.id.drawer_layout);
        if (drawer.isDrawerOpen(GravityCompat.START)) {
            drawer.closeDrawer(GravityCompat.START);
        } else {
            super.onBackPressed();
        }
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        // Inflate the menu; this adds items to the action bar if it is present.
        getMenuInflater().inflate(R.menu.main, menu);
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        // Handle action bar item clicks here. The action bar will
        // automatically handle clicks on the Home/Up button, so long
        // as you specify a parent activity in AndroidManifest.xml.
        int id = item.getItemId();

        //noinspection SimplifiableIfStatement
        if (id == R.id.action_settings) {
            return true;
        }

        return super.onOptionsItemSelected(item);
    }

    @SuppressWarnings("StatementWithEmptyBody")
    @Override
    public boolean onNavigationItemSelected(MenuItem item) {
        // Handle navigation view item clicks here.
        int id = item.getItemId();

        if (id == R.id.nav_camera) {
            // Handle the camera action
        } else if (id == R.id.nav_gallery) {

        } else if (id == R.id.nav_slideshow) {

        } else if (id == R.id.nav_manage) {

        } else if (id == R.id.nav_share) {

        } else if (id == R.id.nav_send) {

        }

        DrawerLayout drawer = (DrawerLayout) findViewById(R.id.drawer_layout);
        drawer.closeDrawer(GravityCompat.START);
        return true;
    }


}
