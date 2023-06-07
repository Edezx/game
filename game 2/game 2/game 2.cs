using System;
using System.Collections.Generic;
using System.Threading;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;
using Timer = Jypeli.Timer;

namespace game_2;

public class game_2 : PhysicsGame
{
    
    
    
    
    private const double NOPEUS = 200;
    private const double HYPPYNOPEUS = 750;
    private const int RUUDUN_KOKO = 40;

    private PlatformCharacter pelaaja1;
    IntMeter pistelaskuri;
    LaserGun pelaajan1Ase;
    private Image pelaajankuva = LoadImage("pepes.png");
    private Image vihollisenkuva = LoadImage("vihollinen.png");

    private SoundEffect maaliAani = LoadSoundEffect("maali.wav");
    
    public override void Begin()
    {
        ClearAll();
        MultiSelectWindow alkuvalikko = new MultiSelectWindow("Pelin alkuvalikko", "Aloita peli", "Lopeta");
        
        alkuvalikko.AddItemHandler(0, AloitaPeli);
        alkuvalikko.AddItemHandler(1, Exit);
        Add(alkuvalikko);
        SetWindowSize(1080, 900, false); 
        
    }

    private void AloitaPeli()
    {
        LuoPistelaskuri();
        LuoKentta();
        LisaaNappaimet();
        Camera.Follow(pelaaja1);
        Camera.ZoomFactor = 1.2;
        Camera.StayInLevel = true;
        Timer.SingleShot(150,PeliLoppuu);
        

    }

    private void PeliLoppuu()
    {
        Label tekstikentta = new Label();
        tekstikentta.Text="Peli loppui hävisit.";
        Add(tekstikentta);

        Timer.SingleShot(5,Begin);
      
    }
    private void LuoKentta()
    {
        TileMap kentta = TileMap.FromLevelAsset("kentta1.txt");
        kentta.SetTileMethod('#', LisaaTaso);
        kentta.SetTileMethod('m', Lisaavihollinen);
        kentta.SetTileMethod('N', LisaaPelaaja);
        kentta.Execute(RUUDUN_KOKO, RUUDUN_KOKO);
        Level.CreateBorders();
        Level.Background.CreateGradient(Color.MediumBlue, Color.Black);
        IsFullScreen = true;
        Gravity = new Vector(0, -1000);
        MasterVolume = 0.5;
    }

    private void LisaaTaso(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject taso = PhysicsObject.CreateStaticObject(leveys, korkeus);
        taso.Position = paikka;
        taso.Color = Color.BloodRed;
        Add(taso);
    }

    private void Lisaavihollinen(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject vihollinen = PhysicsObject.CreateStaticObject(leveys, korkeus);
        vihollinen.IgnoresCollisionResponse = true;
        vihollinen.Position = paikka;
       vihollinen.Image = vihollisenkuva;
        vihollinen.Tag = "vihollinen";
        Add(vihollinen);
        pistelaskuri.AddValue(1);
    }

    private void LisaaPelaaja(Vector paikka, double leveys, double korkeus)
    {
        pelaaja1 = new PlatformCharacter(leveys, korkeus);
        pelaaja1.Position = paikka;
        pelaaja1.Mass = 4.0;
        pelaaja1.Image = pelaajankuva;
        //AddCollisionHandler(pelaaja1, "vihollinen", );
        Add(pelaaja1);
        pelaajan1Ase = new LaserGun(20, 0);
        pelaajan1Ase.Ammo.Value = 1000;

        pelaajan1Ase.FireRate = 5;
        pelaajan1Ase.Position = pelaaja1.Position +new Vector(30,-5);

        pelaaja1.Weapon = pelaajan1Ase;
        pelaaja1.Weapon.ProjectileCollision = AmmusOsui;
        //pelaaja1.Add(pelaajan1Ase);
    }
    void AmmusOsui(PhysicsObject ammus, PhysicsObject kohde)
    {
        ammus.Destroy();
        kohde.Destroy();
        pistelaskuri.AddValue(-1);
    }
    private void LisaaNappaimet()
    {
        Keyboard.Listen(Key.Space, ButtonState.Down, AmmuAseella, "Ammu", pelaajan1Ase);
        Keyboard.Listen(Key.F1, ButtonState.Pressed, ShowControlHelp, "Näytä ohjeet");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");

        Keyboard.Listen(Key.A, ButtonState.Down, Liikuta, "Liikkuu vasemmalle", pelaaja1, -NOPEUS);
        Keyboard.Listen(Key.D, ButtonState.Down, Liikuta, "Liikkuu vasemmalle", pelaaja1, NOPEUS);
        Keyboard.Listen(Key.W, ButtonState.Pressed, Hyppaa, "Pelaaja hyppää", pelaaja1, HYPPYNOPEUS);
        Mouse.Listen(MouseButton.Left, ButtonState.Pressed, AmmuAseella, "Ammu", pelaajan1Ase);
        

        

        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
    }

    private void Liikuta(PlatformCharacter hahmo, double nopeus)
    {
        hahmo.Walk(nopeus);
        if (nopeus < 0)
        {
            hahmo.Turn(Direction.Left);
        }else if (nopeus >0)
        {
            hahmo.Turn(Direction.Right);
        }
    }

    private void Hyppaa(PlatformCharacter hahmo, double nopeus)
    {
        hahmo.Jump(nopeus);
    }
    void AmmuAseella(LaserGun ase)
    {
        PhysicsObject ammus = ase.Shoot();
        if(ammus != null)
        {
            //ammus.Size *= 4;
            //ammus.Image = ...
            //ammus.MaximumLifetime = TimeSpan.FromSeconds(2.0);
        }
        }

    void LuoPistelaskuri()
    {
        pistelaskuri = new IntMeter(0);               
      
        Label pistenaytto = new Label(); 
        pistenaytto.X = Screen.Left + 100;
        pistenaytto.Y = Screen.Top - 100;
        pistenaytto.TextColor = Color.Black;
        pistenaytto.Color = Color.Blue;
        pistenaytto.Title = "Vihollisia jäljellä.";
        pistenaytto.BindTo(pistelaskuri);
        Add(pistenaytto);
    }
}