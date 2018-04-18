using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShaceShip
{
    struct Position
    {
        public int col;
        public int rol;
        public Position(int Col, int Rol)
        {
            this.col = Col;
            this.rol = Rol;
        }
    }


    class Program
    {
        static Position ship = new Position(15, 38);
        static List<Position> bullets = new List<Position>();
        static List<Position> enemies = new List<Position>();
        static Random enemy = new Random();
        static int destroiedEnemies = 0;
        static int ships = 3;
        
        static void Main(string[] args)
        {
            Console.SetWindowSize(50, 40);
            Console.SetBufferSize(50, 40);
            DrawStaticContent();
            int counter = 0;

            
                var t1 = new Thread(() =>
                {
                    while (ships > 0)
                    {
                        UpdateEnemies();
                        RemoveDestroyedShip();
                        Thread.Sleep(100);

                    }
                });

                var t2 = new Thread(() =>
                {
                    while (ships > 0)
                    {
                        UpdateField();
                        UpdateScores();

                        if (counter == 20)
                        {
                            GenerateEnemies();
                            counter = 0;
                        }
                        while (Console.KeyAvailable)
                        {
                            ConsoleKeyInfo userKey = Console.ReadKey();

                            if ((userKey.Key == ConsoleKey.LeftArrow) && ship.col > 1)
                            {
                                ship = new Position(ship.col - 1, ship.rol);
                            }

                            if ((userKey.Key == ConsoleKey.RightArrow) && ship.col < 28)
                            {
                                ship = new Position(ship.col + 1, ship.rol);
                            }

                            if (userKey.Key == ConsoleKey.Spacebar)
                            {
                                bullets.Add(new Position(ship.col, ship.rol - 1));
                            }
                        }

                        Draw();
                        Thread.Sleep(20);
                        counter++;
                    }
                });

                t1.Start();
                t2.Start();
                t1.Join();
                t2.Join();
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(10, 10);
            Console.Write("GAME OVER");
            Console.Read();
            
           
        }

        private static void UpdateScores()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(42, 10);
            Console.Write(destroiedEnemies);
            Console.SetCursorPosition(42, 12);
            Console.Write(ships);
        }

        private static void GenerateEnemies()
        {
            enemies.Add(new Position(enemy.Next(1,28), 1));
            //enemies.Add(new Position(15, 1));
        }

        private static void UpdateField()
        {
            RemoveDestroyedEnmy();
            UpdateShots();
            //RemoveDestroyedShip();
        }

        private static void RemoveDestroyedShip()
        {
            lock (enemies)
            {
                foreach (var item in enemies)
                {
                    if ((ship.rol == item.rol) && (ship.col == item.col))
                    {
                        ships--;
                    }

                }
            }
            
            
        }

        private static void RemoveDestroyedEnmy()
        {
            if (enemies.Count >= 1)
            {
                for (int i = 0; i < bullets.Count; i++)
                {
                    for (int j = 0; j < enemies.Count; j++)
                    {
                        if ((bullets[i].col == enemies[j].col) && (enemies[j].rol >= bullets[i].rol))
                        {
                            enemies.Remove(enemies[j]);
                            destroiedEnemies++;
                            if (enemies.Count == 0)
                            {
                                break;
                            }
                        }
                    }
                }
            }

        }

        private static void UpdateEnemies()
        {
            lock (enemies)
            {
                if (enemies.Count != 0)
                {
                    for (int i = 0; i <= (enemies.Count - 1); i++)
                    {
                        enemies[i] = new Position(enemies[i].col, enemies[i].rol + 1);
                        if (enemies[i].rol > Console.WindowHeight - 2)
                        {
                            enemies.Remove(enemies[i]);
                        }
                    }
                }
            }

        }

        private static void UpdateShots()
        {
            if (bullets.Count != 0)
            {
                for (int i = (bullets.Count - 1); i >= 0; i--)
                {
                    bullets[i] = new Position(bullets[i].col, bullets[i].rol - 1);
                    if (bullets[i].rol < 1)
                    {
                        bullets.Remove(bullets[i]);
                    }
                }
            }

        }

        static void Draw()
        {
            DrawShip();
            DrawEnemy();
            DrawShot();
        }
       
        private static void DrawStaticContent()
        {
            for (int j = 1; j < Console.WindowHeight - 1; j++)
            {
                for (int i = 0; i < 30; i+=29)
                {
                    DrawSymbolAtPosition(i, j, '*', ConsoleColor.Green);
                }

            }

            for (int i = 0; i < Console.WindowHeight; i+=Console.WindowHeight -1)
            {
                for (int j = 0; j < 30; j++)
                {
                    Console.SetCursorPosition(j, i);
                    Console.Write("*");
                }
            }
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.SetCursorPosition(35, 10);
            Console.Write($"{"SCORE: "}");
            Console.SetCursorPosition(35, 12);
            Console.Write($"{"SHIPS: "}");

        }

        private static void DrawEnemy()
        {
            lock (enemies)
            {
                foreach (var item in enemies)
                {
                    DrawSymbolAtPosition(item.col, item.rol, '@', ConsoleColor.Magenta);
                    if (item.rol >= 2 && item.rol < Console.WindowHeight)
                    {
                        if (item.rol == Console.WindowHeight - 2)
                        {
                            DrawSymbolAtPosition(item.col, item.rol, ' ', ConsoleColor.Black);
                        }
                        DrawSymbolAtPosition(item.col, item.rol - 1, ' ', ConsoleColor.Black);
                    }
                }
            }
        }

        private static void DrawShot()
        {
            foreach (var item in bullets)
            {
                DrawSymbolAtPosition(item.col, item.rol, '|', ConsoleColor.Yellow);
                if (item.rol >= 1)
                {
                    if (item.rol == 1)
                    {
                        DrawSymbolAtPosition(item.col, item.rol, ' ', ConsoleColor.Black);
                    }
                    DrawSymbolAtPosition(item.col, item.rol + 1, ' ', ConsoleColor.Black);
                }
            }
        }

        private static void DrawShip()
        {
            if (ship.col >= 1 && ship.col < 29)
            {

                for (int i = ship.col - 1; i > 0; i--)
                {
                    DrawSymbolAtPosition(i, ship.rol, '#', ConsoleColor.Red);
                }

                for (int i = ship.col + 1 ; i < 29; i++)
                {
                    DrawSymbolAtPosition(i, ship.rol, '#', ConsoleColor.Blue);
                }
            }

            DrawSymbolAtPosition(ship.col, ship.rol, '#', ConsoleColor.White);
        }

        static void DrawSymbolAtPosition(int col, int rol, char symbol, ConsoleColor color)
        {
            Console.SetCursorPosition(col, rol);
            Console.ForegroundColor = color;
            Console.Write(symbol);
        }
    } 
}
