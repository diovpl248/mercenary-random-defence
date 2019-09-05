용병 랜덤 디펜스 (소스 참고용)
---

![메인화면](https://github.com/diovpl248/mercenary-random-defence/blob/master/game.png?raw=true)

![인게임](https://github.com/diovpl248/mercenary-random-defence/blob/master/in-game.PNG?raw=true)

YouTube: https://youtu.be/KK5aYnqKmR0

---

* [게임 데이터] 유닛기본스텟, 토템, 유물, 조합식 등 json파일에서 읽어옴. JsonManager.cs, (Resources\Json폴더) 
* [타원] Ellipse.cs
* [캐릭터] Character.cs
* [이동] Movement.cs
* [공격] Attack.cs
* [특수 스킬 공격] Magic.cs, ExplosionParticle.cs
* [토템] TotemManager.cs, Totem.cs
* [유물] ArtifactManager.cs
* [버프 디버프] 공격력상승, 방어력상승, 힐, 도트데미지, 슬로우 등을 적용 BuffSystem.cs
* [최적화] 유닛, 데미지폰트, 스킬이펙트등 오브젝트 풀링으로 재사용 MasterFloatingTextPool.cs, MasterObjectPool.cs
* [옵저버 패턴을 활용 c# delegate] QuestEventManager.cs
* [세이브 로드] 바이너리 파일로 세이브 및 로드 SaveSysttem.cs
* [UI] Async씬로딩 Loading.cs