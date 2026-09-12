# Anúbis — Action Roguelite 2D

Vertical slice jogável de um action roguelite top-down para PC/Steam.

Motor alvo: **Unity 6.3 LTS (6000.3.x)** e C#.

## Como abrir

1. Instale o [Unity Hub](https://unity.com/download) e o editor **Unity 6.3 LTS**.
2. Em *Add* → *Add project from disk*, escolha esta pasta.
3. Abra a cena `Assets/Scenes/AnubisArena.unity`.
4. Aperte Play.

O slice sobe sozinho: movimento, câmera, Khopesh, dash, vida, três inimigos, arena, portas, santuário e escolha de bênçãos.

Opcional no editor: menu **Anubis → Create Default Data Assets** para gravar os ScriptableObjects em disco.

## Controles

| Ação | Teclado / mouse | Controle |
| --- | --- | --- |
| Mover | WASD ou setas | Analógico esquerdo |
| Mirar | Mouse | Analógico direito |
| Khopesh | Clique esquerdo ou J | X / quadrado ou RT |
| Dash | Espaço ou Shift | B / círculo ou LB |
| Interagir | E | Y / triângulo |
| Escolher bênção | 1 / 2 / 3, setas + Enter | D-pad + A / X |

## Vertical slice

1. Movimentação top-down com aceleração.
2. Câmera ortográfica que segue Anúbis e respeita a arena.
3. Ataque em arco com o Khopesh.
4. Dash com i-frames e cooldown.
5. Vida, dano, knockback e invulnerabilidade.
6. Três inimigos com state machine: Escaravelho, Guardião da Tumba, Arqueiro Chacal.
7. Arena modular montada a partir de `RoomDefinition`.
8. Portas que fecham no combate e abrem ao limpar a sala.
9. Santuário de recompensa após o encontro.
10. Oferta de três bênçãos desacopladas via eventos.

Há também save JSON local, progressão permanente (+5 HP a cada 3 arenas) e um gate pronto para Steamworks.

## Arquitetura

```
Assets/
├── Art/
├── Audio/
├── Prefabs/
├── Scenes/
├── ScriptableObjects/
│   ├── Characters/
│   ├── Blessings/
│   ├── Enemies/
│   └── Relics/
└── Scripts/
    ├── Core/          eventos, pooling, input, bootstrap
    ├── Combat/        vida, dano, projéteis
    ├── Characters/    Anúbis, motor, dash, Khopesh, câmera
    ├── AI/            state machine e inimigos
    ├── Rooms/         salas, portas, encontro, run procedural
    ├── Progression/   bênçãos e relíquias
    ├── Save/          save e meta
    ├── Platform/      local agora, Steam depois
    └── UI/
```

Dados ficam em ScriptableObjects. Sistemas se falam por `GameSignals`, não por referências rígidas. Bênçãos assinam eventos (`EnemyKilled`, etc.) em vez de conhecer o combate por dentro.

`ProceduralRunBuilder` já escolhe salas pré-criadas de um pool. Neste slice o run tem uma arena só.

## Próximos passos (ainda não implementados)

- Loop completo de run com grafo de salas
- Mais personagens e chefes
- Relíquias e loja de meta
- Arte, áudio e Cinemachine
- Integração Steamworks.NET (achievements, cloud, overlay)
