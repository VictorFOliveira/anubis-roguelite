# Anubis Runner

Endless runner 2D em Unity 6 focado no Anúbis atravessando biomas inspirados no Egito e Mesopotâmia enquanto Rá o persegue do céu.

## Controles

- `A` / `←`: recuar um pouco para esquivar de projéteis;
- `D` / `→`: avançar / ganhar velocidade;
- `Espaço`, `W` ou `↑`: pular;
- `S`, `↓` ou `Ctrl`: deslizar;
- `↓` no ar: queda rápida;
- `R` / `Start`: reiniciar depois de morrer.

O recuo é limitado para preservar o ritmo de runner. A câmera continua avançando e a distância exibida usa a maior distância já alcançada, então recuar não reduz o contador.

## Distância

A escala de distância foi reduzida para **0,5 m por unidade de mundo**, deixando metros e quilômetros bem mais lentos e legíveis durante a corrida.

## Movimento

- corrida automática com controle horizontal leve;
- pulo com coyote time, jump buffer e altura variável;
- fast-fall;
- slide com collider baixo e duração mínima para a pose ficar visível;
- stomp em inimigos;
- squash/stretch, poeira, inclinação e sombra dinâmica.

## Sprites

Os sprites usados em runtime ficam em `Assets/Resources/Runner/`:

- `anubis_idle.png`
- `anubis_walk.png`
- `anubis_run.png`
- `anubis_jump.png`
- `anubis_slide.png`
- `ra_cloud_idle.png`
- `ra_cloud_cast.png`
- `ra_orb.png`

O slide usa uma pose longa e baixa, escalada pela largura do sprite, e mantém o collider reduzido enquanto Anúbis estiver sob obstáculos baixos.

## Cena

Abra `Assets/Scenes/AnubisRunner.unity` e execute Play.
